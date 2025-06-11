using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public class ItemRepository : Repository<Guid, Item>
    {
        public ItemRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<Item> Get(Guid key)
        {
            var item = await _auctionContext.Items.Where(i => i.IsDeleted == false)
                                                    .Include(i => i.ItemDetails)
                                                    .SingleOrDefaultAsync(p => p.Id == key);

            return item ?? throw new IdNotFoundException("No Item found with the given ID");
        }

        public override async Task<IEnumerable<Item>> GetAll()
        {
            var items = await _auctionContext.Items.Where(i => i.Status == "Active").Include(i => i.ItemDetails).ToListAsync();
            if (items.Count() == 0)
                throw new CollectionEmptyException("No Items in the database");
            return items;
        }

    }
}