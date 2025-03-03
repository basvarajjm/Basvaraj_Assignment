using WebApp.Models;

namespace WebApp.Interfaces
{
    public interface IDKBankService
    {
        Task<List<Currency>?> GetCurrencyExchangeRates(CancellationToken cancellationToken = default);
    }
}
