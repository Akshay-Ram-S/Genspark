namespace BankAPI.Models.Dtos
{
    public class CustomerAddRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly Dob { get; set; }
        public string Email { get; set; } = string.Empty;
        public required string Pan { get; set; }
    }
}