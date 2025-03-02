using WebApp.Interfaces;

namespace WebApp.Services
{
    public class CurrencyService: ICurrenyService
    {

        public double GetDKKEquivalentOf(string currency, long value)
        {
            // Repository call
            double rate = 12;

            double result;
            checked
            {
                result = value * rate;
            }
            return result;
        }
    }
}
