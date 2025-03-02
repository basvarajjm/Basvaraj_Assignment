namespace WebApp.Interfaces
{
    public interface ICurrenyService
    {
        double GetDKKEquivalentOf(string currency, long value);
    }
}
