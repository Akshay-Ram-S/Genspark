using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models
{
    public class Seller
    {
        [Key]
        public Guid SellerId { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Item>? Items { get; set; }
        public User? User { get; set; }

    }
}