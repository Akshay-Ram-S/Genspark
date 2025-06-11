using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models
{
    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? Category { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public Guid SellerID { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Collection<Bid>? Bids { get; set; }
        public Seller? Seller{ get; set; }
        public ItemDetails? ItemDetails { get; set; }
    }
}
