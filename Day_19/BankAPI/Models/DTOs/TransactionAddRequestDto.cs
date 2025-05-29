namespace BankAPI.Models.Dtos
{
    public class TransactionAddRequestDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string ActionType { get; set; } = string.Empty;
    }
}