namespace AuctionAPI.Models.DTOs
{
    public class UpdateUserDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}