using WebApp.Models;

namespace WebApp.Interfaces
{
    public interface IRepository
    {
        Task<List<CurrencyRate>> GetCurrencyRates();
    }
}
