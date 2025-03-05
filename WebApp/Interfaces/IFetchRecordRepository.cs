using WebApp.Models;

namespace WebApp.Interfaces
{
    public interface IFetchRecordRepository
    {
        Task StoreCurrencyRecordData(ConvertedRecord convertedRecord, CancellationToken cancellationToken = default);
        Task<List<ConvertedRecord>> GetCurrencyRecordData(DateTime from, DateTime to, string currency, int offset, int limit, CancellationToken cancellationToken = default);
    }
}
