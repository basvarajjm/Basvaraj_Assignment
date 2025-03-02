namespace WebApp.Interfaces
{
    public interface ICurrenyService
    {
        long GetDKKEquivalentOf(string currency, long value);
    }
}
