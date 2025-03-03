using System.Xml.Serialization;

namespace WebApp.Models
{
    [Serializable]
    public class Currency
    {
        [XmlAttribute("code")]
        public required string Code { get; set; }

        [XmlAttribute("desc")]
        public required string Desc { get; set; }

        [XmlAttribute("rate")]
        public double Rate { get; set; }
    }

    [XmlRoot("exchangerates")]
    public class ExchangeRates
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("author")]
        public string Author { get; set; }

        [XmlAttribute("refcur")]
        public string RefCur { get; set; }

        [XmlAttribute("refamt")]
        public int RefAmt { get; set; }

        [XmlElement("dailyrates")]
        public DailyRates DailyRates { get; set; }
    }

    public class DailyRates
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("currency")]
        public List<Currency> Currencies { get; set; }
    }
}
