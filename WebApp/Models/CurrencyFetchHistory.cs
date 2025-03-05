namespace WebApp.Models
{
    public class CurrencyFetchHistory
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
