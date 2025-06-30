namespace AuctionAPI.Models.DTOs
{
    public class ItemAllBids
    {
        public Guid BidId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid Bidder_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime bid_timestamp { get; set; }
    }

}