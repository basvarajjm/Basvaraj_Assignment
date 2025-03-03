using System.Xml.Serialization;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Services
{
    public class DKBankService : IDKBankService
    {
        private readonly ILogger<DKBankService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public DKBankService(ILogger<DKBankService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _baseUrl = configuration.GetValue<string>("DKBank:BaseUrl") ?? "";
        }

        public async Task<List<Currency>?> GetCurrencyExchangeRates(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/api/currencyratesxml?lang=en-us", cancellationToken);

            return await EnsureSuccessStatus(response);
        }

        private async Task<List<Currency>?> EnsureSuccessStatus(HttpResponseMessage httpResponse)
        {
            ExchangeRates? exchangeRates = null;
            try
            {
                httpResponse.EnsureSuccessStatusCode();

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //XmlRootAttribute xRoot = new XmlRootAttribute();
                    //xRoot.ElementName = "exchangerates";
                    //xRoot.IsNullable = true;
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExchangeRates));
                    string s = await httpResponse.Content.ReadAsStringAsync();
                    StringReader content = new StringReader(s);
                    exchangeRates = (ExchangeRates?) xmlSerializer.Deserialize(content);
                    return exchangeRates?.DailyRates.Currencies;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching DK Bank records");
            }
            return exchangeRates?.DailyRates.Currencies;
        }
    }
}
