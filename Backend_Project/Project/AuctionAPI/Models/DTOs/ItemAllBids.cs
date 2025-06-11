namespace AuctionAPI.Models.DTOs
{
    public class ItemAllBids
    {
        public string title { get; set; } = string.Empty;
        public Guid bidder_id { get; set; }
        public string name { get; set; } = string.Empty;
        public decimal amount { get; set; }
        public DateTime bid_timestamp { get; set; }
    }

}