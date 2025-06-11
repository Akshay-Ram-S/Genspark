using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.AspNetCore.Components.Web;

namespace AuctionAPI.Mappers
{
    public class BidResponseMapper
    {
        public BidResponse? MapBidResponse(Bid bid, Bidder bidder, Item item)
        {
            var bidResponse = new BidResponse
            {
                Id = bid.Id,
                ItemID = bid.ItemId,
                Title = item.Title,
                BidderName = bidder.User.Name?? "Name not found",
                Amount = bid.Amount,
                Timestamp = bid.Timestamp,
                Status = bid.Status
            };
            return bidResponse;
        }
    }
}
