namespace BankAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } = string.Empty;

        public Account? Account { get; set; }
    }

}