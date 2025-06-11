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
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = dto.EndDate,
                SellerID = dto.SellerID
            };
            return item;
        }
    }
}