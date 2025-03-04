using WebApp.Models;

namespace WebApp.Interfaces
{
    public interface ICurrenyConversionService
    {
        Task<double> GetDKKEquivalentOf(string currency, long value, CancellationToken cancellationToken = default);
        Task<List<Currency>> GetAllCurrenciesAsync(CancellationToken cancellationToken = default);
    }
}
