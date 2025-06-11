using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models
{
    public class Bidder
    {
        [Key]
        public Guid BidderId { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Bid>? Bids { get; set; }
        public User? User { get; set; }

    }
}