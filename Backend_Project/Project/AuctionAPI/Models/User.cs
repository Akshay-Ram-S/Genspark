using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PAN{get; set; } = string.Empty;
        public string Aadhar { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Seller? Seller { get; set; }
        public Bidder? Bidder { get; set; }

    }
}