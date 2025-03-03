using WebApp.Models;

namespace WebApp.Interfaces
{
    public interface IRepository
    {
        Task<List<Currency>?> GetCurrencyRatesAsync(CancellationToken cancellationToken = default);
        Task CreateOrUpdateCurrencyAsync(List<Currency> list, CancellationToken cancellationToken = default);
    }
}
