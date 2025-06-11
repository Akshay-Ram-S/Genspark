using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models
{
    public class Bid
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid BidderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public Bidder? Bidder { get; set; }
        public Item? Item { get; set; }
    }
}