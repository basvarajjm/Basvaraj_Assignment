using System.Text.Json;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Repositories
{
    public class FileRepository : IRepository
    {
        private readonly ILogger<FileRepository> _logger;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public FileRepository(ILogger<FileRepository> logger)
        {
            _logger = logger;
        }
        private const string _currencyFile = "./Currency.json";
        public async Task<List<Currency>?> GetCurrencyRatesAsync(CancellationToken cancellationToken = default)
        {
            List<Currency>? list = null;
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (File.Exists(_currencyFile))
                {
                    string data = await File.ReadAllTextAsync(_currencyFile, cancellationToken);
                    list = JsonSerializer.Deserialize<List<Currency>?>(data);
                }
            }
            finally
            {
                semaphore.Release();
            }
            return list;
        }

        public async Task CreateOrUpdateCurrencyAsync(List<Currency> list, CancellationToken cancellationToken = default)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                string dataSerialized = JsonSerializer.Serialize(list);
                await File.WriteAllTextAsync(_currencyFile, dataSerialized, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        }

    }
}
