using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public class BidRepository : Repository<Guid, Bid>
    {
        public BidRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<Bid> Get(Guid key)
        {
            var bid = await _auctionContext.Bids.Where(b => b.IsDeleted == false)
                                                .Include(b => b.Bidder)
                                                .SingleOrDefaultAsync(p => p.Id == key);

            return bid??throw new IdNotFoundException("No Bid found with the given ID");
        }

        public override async Task<IEnumerable<Bid>> GetAll()
        {
            var bids = _auctionContext.Bids.Where(b => b.IsDeleted == false).Include(b => b.Bidder);
            return await bids.ToListAsync();
        }
    }
}