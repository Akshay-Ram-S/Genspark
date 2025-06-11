using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.VisualBasic;

namespace AuctionAPI.Mappers
{
    public class BidMapper
    {
        public Bid? MapBid(BidCreateDTO bidDto)
        {
            var bid = new Bid
            {
                ItemId = bidDto.ItemId,
                BidderId = bidDto.BidderId,
                Amount = bidDto.Amount,
                Timestamp = DateTime.UtcNow,
                Status = "Active"
            };
            
            return bid;
        }
    }
}