using Coravel.Invocable;
using WebApp.Interfaces;
using WebApp.Repositories;

namespace WebApp.BackgroundTasks
{
    public class UpdateCurrenyJob : IInvocable, ICancellableInvocable
    {
        private readonly ILogger<UpdateCurrenyJob> _logger;
        private readonly IRepository _cacheRepository;
        private readonly IRepository _fileRepository;
        private readonly IDKBankService _dKBankService;
        public CancellationToken CancellationToken { get; set; }

        public UpdateCurrenyJob(ILogger<UpdateCurrenyJob> logger, IServiceProvider serviceProvider, IDKBankService dKBankService)
        {
            _logger = logger;
            _cacheRepository = serviceProvider.GetRequiredService<CacheRepository>();
            _fileRepository = serviceProvider.GetRequiredService<FileRepository>();
            _dKBankService = dKBankService;
        }

        public async Task Invoke()
        {
            //if (CancellationToken.IsCancellationRequested)
            //{
            //    return;
            //}
            _logger.LogInformation("Started Job.");
            int retries = 3;
            while (true)
            {
                try
                {
                    var list = await _dKBankService.GetCurrencyExchangeRates(CancellationToken);
                    if (list != null && list.Count > 0)
                    {
                        await _cacheRepository.CreateOrUpdateCurrencyAsync(list, CancellationToken);
                        await _fileRepository.CreateOrUpdateCurrencyAsync(list, CancellationToken);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong in scheduled task, with remaining count {retries}.");
                }
                if (--retries < 0)
                {
                    break;
                }
            }
            _logger.LogInformation("Finised Job.");
        }
    }
}
