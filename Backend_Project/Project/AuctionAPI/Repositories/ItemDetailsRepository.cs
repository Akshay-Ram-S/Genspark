using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public class ItemDetailsRepository : Repository<Guid, ItemDetails>
    {
        public ItemDetailsRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<ItemDetails> Get(Guid key)
        {
            var item = await _auctionContext.ItemDetails.SingleOrDefaultAsync(p => p.ItemId == key);

            return item??throw new IdNotFoundException("No Item Details found with the given ID");
        }

        public override async Task<IEnumerable<ItemDetails>> GetAll()
        {
            var items = _auctionContext.ItemDetails;
            if (items.Count() == 0)
                throw new CollectionEmptyException("No Items in the auction");
            return await items.ToListAsync();
        }
    }
}