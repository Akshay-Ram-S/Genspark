using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Interfaces
{
    public interface IItemService
    {
        public Task<ItemResponse> CreateItemAsync(ItemCreateDto dto, string email);
        public Task<ItemResponse> GetItemById(Guid id);
        public Task<PagedResult<ItemResponse>> GetItems(ItemFilter filter, int page = 1, int pageSize = 10);
        public Task<Item> DeleteItem(Guid id, string userEmail, string role);
        public Task<ItemResponse> UpdateItem(Guid id, ItemUpdateDto dto, string userEmail, string role);
    }
}