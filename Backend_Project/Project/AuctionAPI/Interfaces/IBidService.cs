using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Interfaces
{
    public interface IBidService
    {
        public Task<BidResponse> PlaceBid(BidCreateDTO bidDto, string role);
        public Task<BidResponse> GetBidById(Guid id);
        public Task<Bid> CancelBid(Guid id);
    }
}