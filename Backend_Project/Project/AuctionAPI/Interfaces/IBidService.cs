using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Interfaces
{
    public interface IBidService
    {
        public Task<BidResponse> PlaceBid(BidCreateDTO bidDto);
        public Task<BidResponse> GetBidById(Guid id);
        public Task<BidResponse> CancelBid(Guid id, string email);
    }
}