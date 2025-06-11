namespace AuctionAPI.Models.DTOs
{
    public class UserLoginResponse
    {
        public string Email { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}