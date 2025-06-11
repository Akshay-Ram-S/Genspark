namespace AuctionAPI.Models.DTOs
{
    public class ItemsBySellerDto
    {
        public Guid ItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}