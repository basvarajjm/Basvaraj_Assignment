using WebApp.Interfaces;

namespace WebApp.Services
{
    public class CurrencyService: ICurrenyService
    {

        public long GetDKKEquivalentOf(string currency, long value)
        {
            // Repository call
            long rate = 12;

            long result;
            checked
            {
                result = value * rate;
            }
            return result;
        }
    }
}
