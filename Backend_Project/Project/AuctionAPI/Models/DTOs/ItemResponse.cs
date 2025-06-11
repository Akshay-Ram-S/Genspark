namespace AuctionAPI.Models.DTOs
{
    public class ItemResponse
    {
        public Guid ItemID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal? CurrentBid { get; set; }
        public string CurrentBidderName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        
    }
}