using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Repository
{
    public class FileRepository : IRepository
    {
        public async Task<List<CurrencyRate>> GetCurrencyRates()
        {
            //Read from file and return the list.
            throw new NotImplementedException(); 
        }
        
    }
}
