using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuctionAPI.Services;

namespace AuctionAPI.Models
{
    public class ItemDetails
    {
        [ForeignKey("Item")]
        public Guid ItemId { get; set; }
        public decimal StartingPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public byte[]? ImageData { get; set; }
        public string ImageMimeType { get; set; } = string.Empty;
        public decimal CurrentBid { get; set; }
        public Guid? CurrentBidderID { get; set; }

        public Item? Item { get; set; }
        public Bidder? Bidder{ get; set; }
    }
}
