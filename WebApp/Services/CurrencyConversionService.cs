using WebApp.Exceptions;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Services
{
    public class CurrencyConversionService: ICurrenyConversionService
    {
        private readonly ILogger<CurrencyConversionService> _logger;
        private readonly IRepository _cacheRepository;
        private readonly IRepository _fileRepository;
        private readonly IDKBankService _dKBankService;

        public CurrencyConversionService(ILogger<CurrencyConversionService> logger, IServiceProvider serviceProvider, IDKBankService dKBankService)
        {
            _logger = logger;
            _cacheRepository = serviceProvider.GetRequiredService<CacheRepository>();
            _fileRepository = serviceProvider.GetRequiredService<FileRepository>();
            _dKBankService = dKBankService;
        }

        public async Task<double> GetDKKEquivalentOf(string currency, long value, CancellationToken cancellationToken = default)
        {
            var list = await GetCurrenciesAsync(cancellationToken);
            var data = list.Find(x => x.Code.ToLower() == currency.ToLower());
            if (data == null)
            {
                throw new ItemNotFoundException();
            }
            double rate = data.Rate;
            double result;
            checked
            {
                result = value * rate;
            }
            return result;
        }

        public async Task<List<Currency>> GetAllCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCurrenciesAsync(cancellationToken);
        }

        private async Task<List<Currency>> GetCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _cacheRepository.GetCurrencyRatesAsync(cancellationToken);
            if (list != null && list.Count > 0)
            {
                _logger.LogInformation("Cache hit.");
                return list;
            }
            list = await _fileRepository.GetCurrencyRatesAsync(cancellationToken);
            if (list != null && list.Count > 0)
            {
                _logger.LogInformation("Data found from file.");
                return list;
            }
            return await GetCurrencyDataFromDKKBankService(cancellationToken);
        }

        private async Task<List<Currency>> GetCurrencyDataFromDKKBankService(CancellationToken cancellationToken = default)
        {
            var list = await _dKBankService.GetCurrencyExchangeRates(cancellationToken);
            if (list != null && list.Count > 0)
            {
                await _cacheRepository.CreateOrUpdateCurrencyAsync(list, cancellationToken);
                await _fileRepository.CreateOrUpdateCurrencyAsync(list, cancellationToken);
            }
            return list;
        }
    }
}
