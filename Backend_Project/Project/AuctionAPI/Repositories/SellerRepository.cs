using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public  class SellerRepository : Repository<Guid, Seller>
    {
        public SellerRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<Seller> Get(Guid key)
        {
            var seller = await _auctionContext.Sellers.Where(s => s.User.Status == "Active")
                                                        .Include(s => s.User)
                                                        .SingleOrDefaultAsync(p => p.SellerId == key);

            return seller??throw new IdNotFoundException("No Active Seller with the given ID");
        }

        public override async Task<IEnumerable<Seller>> GetAll()
        {
            var sellers = _auctionContext.Sellers.Where(s => s.User.Status == "Active").Include(s=>s.User);
            if (sellers.Count() == 0)
                throw new CollectionEmptyException("No Active Sellers in the database");

            return await sellers.ToListAsync();
        }
    }
}