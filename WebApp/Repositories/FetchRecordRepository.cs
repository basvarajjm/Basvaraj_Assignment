using System.Text.Json;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Repositories
{
    public class FetchRecordRepository : IFetchRecordRepository
    {
        private const string _file = "./Curreny_Conversion";

        private const string _fileExtension = ".json";

        private string _writeFileName
        {
            get
            {
                return _file + DateTime.Now.ToString("ddMMyyyy") + _fileExtension;
            }
        }

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task<List<ConvertedRecord>> GetCurrencyRecordData(DateTime from, DateTime to, string currency, int offset, int limit, CancellationToken cancellationToken = default)
        {
            List<ConvertedRecord> list = new List<ConvertedRecord>();

            var days = to.Subtract(from);
            int totalFiles = (int)days.TotalDays + 1;

            int skippedRecords = 0;
            for (int i = 0; i < totalFiles; ++i)
            {
                string filename = GetReadFileReadName(from.AddDays(i));

                if (File.Exists(filename))
                {
                    bool isEnteredSemaphore = false;
                    try
                    {
                        var (fs, isLocked) = await CheckFileAvailability(filename, true);
                        isEnteredSemaphore = isLocked;
                        using (fs)
                        {
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                string? data = null;
                                do
                                {
                                    data = await reader.ReadLineAsync(cancellationToken);
                                    if (data != null && skippedRecords >= offset)
                                    {
                                        ConvertedRecord? record = JsonSerializer.Deserialize<ConvertedRecord>(data);
                                        if (record != null && (string.IsNullOrEmpty(currency) ? true : record.Currency == currency))
                                        {
                                            list.Add(record);
                                            if (list.Count >= limit)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    ++skippedRecords;
                                } while (data != null);
                            }
                        }
                    }
                    finally
                    {
                        if (isEnteredSemaphore)
                        {
                            semaphore.Release();
                        }
                    }
                }
            }
            return list;
        }

        public async Task StoreCurrencyRecordData(ConvertedRecord convertedRecord, CancellationToken cancellationToken = default)
        {
            string serializedData = JsonSerializer.Serialize(convertedRecord);
            bool isEnteredSemaphore = false;
            try
            {
                var (fs, isLocked) = await CheckFileAvailability(_writeFileName, false);
                isEnteredSemaphore = isLocked;
                using (fs)
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        await writer.WriteLineAsync(serializedData);
                    }
                }
            }
            finally
            {
                if (isEnteredSemaphore)
                {
                    semaphore.Release();
                }
            }
        }

        private string GetReadFileReadName(DateTime date)
        {
            return _file + date.ToString("ddMMyyyy") + _fileExtension;
        }

        private async Task<(FileStream, bool)> CheckFileAvailability(string fileName, bool isReadRequest)
        {
            bool isEnteredSemaphore = false;
            if (fileName == _writeFileName)
            {
                await semaphore.WaitAsync();
                isEnteredSemaphore = true;
            }
            if (isReadRequest)
            {
                return (new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), isEnteredSemaphore);
            }
            return (new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None), isEnteredSemaphore);
        }
    }
}
