using Microsoft.Extensions.Caching.Memory;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Repositories
{
    public class CacheRepository : IRepository
    {
        private readonly IMemoryCache _memoryCache;
        private const string _dataKey = "DK_Bank";

        public CacheRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public Task CreateOrUpdateCurrencyAsync(List<Currency> list, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_memoryCache.Set<List<Currency>>(_dataKey, list));
        }

        public Task<List<Currency>?> GetCurrencyRatesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_memoryCache.Get<List<Currency>>(_dataKey));
        }
    }
}
