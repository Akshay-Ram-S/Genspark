namespace AuctionAPI.Models.DTOs
{
    public class BidsByBidderDto
    {
        public Guid BidderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid ItemId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}