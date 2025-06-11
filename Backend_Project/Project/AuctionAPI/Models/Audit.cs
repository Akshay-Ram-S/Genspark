namespace AuctionAPI.Models
{
    public class Audit
    {
        public Guid Id{ get; set; } = Guid.NewGuid();
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}