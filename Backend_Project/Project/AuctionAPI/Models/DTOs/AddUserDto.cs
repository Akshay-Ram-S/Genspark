namespace AuctionAPI.Models.DTOs
{
    public class AddUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PAN { get; set; } = string.Empty;
        public string Aadhar { get; set; } = string.Empty;
    }
}