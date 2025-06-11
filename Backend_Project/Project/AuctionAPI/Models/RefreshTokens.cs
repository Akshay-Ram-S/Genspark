namespace AuctionAPI.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } 
        public string Token { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

}