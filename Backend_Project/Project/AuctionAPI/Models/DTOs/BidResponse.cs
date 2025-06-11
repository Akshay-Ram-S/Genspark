namespace AuctionAPI.Models.DTOs
{
    public class BidResponse
    {
        public Guid Id { get; set; }
        public Guid ItemID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; } 
        public string Status { get; set; } = string.Empty;

    }
}