
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Mappers
{
    public class ItemResponseMapper
    {
        public ItemResponse? MapItemResponse(Item item, ItemDetails itemDetail, Seller seller, Bidder? bidder)
        {
            var itemResponse = new ItemResponse
            {
                ItemID = item.Id,
                Title = item.Title,
                Description = itemDetail.Description ?? "No description",
                Status = item.Status,
                StartingPrice = itemDetail.StartingPrice,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Category = item.Category ?? "Uncategorized",
                CurrentBid = itemDetail.CurrentBid,
                CurrentBidderName = bidder?.User?.Name ?? string.Empty,
                SellerName = seller?.User?.Name ?? string.Empty
            };

            return itemResponse;
        }

    }
}