namespace BankAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly Dob { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Pan { get; set; }

        public ICollection<Account>? Accounts { get; set; }
    }
}