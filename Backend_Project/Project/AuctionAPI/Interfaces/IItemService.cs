using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Interfaces
{
    public interface IItemService
    {
        public Task<ItemResponse> CreateItemAsync(ItemCreateDto dto);
        public Task<ItemResponse> GetItemById(Guid id);
        public Task<PagedResult<ItemResponse>> GetItems(ItemFilter filter, int page = 1, int pageSize = 10);
        public Task<Item> DeleteItem(Guid id, string userEmail);
        public Task<ItemResponse> UpdateItem(Guid id, ItemUpdateDto dto, string userEmail);
    }
}