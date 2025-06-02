namespace BankAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public double Balance { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Customer? Customer { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }

}