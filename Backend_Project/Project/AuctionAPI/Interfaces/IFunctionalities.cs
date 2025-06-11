using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Interfaces
{
    public interface IFunctionalities
    {
        public Task<User> RegisterUser(AddUserDto user);
        public Task<IEnumerable<ItemAllBids>> AllBids(Guid id);
        public Task<Item> GetItemWithBids(Guid id);
        public Task<IEnumerable<ItemsBySellerDto>> ItemsBySeller(Guid id);
        public Task<IEnumerable<BidsByBidderDto>> BidsByBidder(Guid bidderId);
    }
}