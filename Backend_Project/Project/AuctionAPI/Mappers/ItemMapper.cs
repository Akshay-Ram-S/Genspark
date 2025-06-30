using System.Security.Claims;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Mappers
{
    public class ItemMapper
    {
        public Item? MapItem(ItemCreateDto dto)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Status = "Active",
                Category = dto.Category,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc),
                SellerID = dto.SellerID,
                BidderID = null
            };
            return item;
        }
    }
}