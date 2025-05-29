namespace BankAPI.Models.Dtos
{
    public class AccountAddRequestDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public double Balance { get; set; }
        public string AccountType { get; set; } = string.Empty;
    }
}