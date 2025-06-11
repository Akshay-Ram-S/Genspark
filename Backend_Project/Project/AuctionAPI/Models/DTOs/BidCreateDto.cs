namespace AuctionAPI.Models.DTOs
{
    public class BidCreateDTO
    {
        public Guid ItemId { get; set; }
        public Guid BidderId { get; set; }
        public decimal Amount { get; set; }
    }
}
