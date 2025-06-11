using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public  class BidderRepository : Repository<Guid, Bidder>
    {
        public BidderRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<Bidder> Get(Guid key)
        {
            var bidder = await _auctionContext.Bidders.Include(b=>b.User).Where(b => b.User.Status == "Active").SingleOrDefaultAsync(p => p.BidderId == key);

            return bidder ?? throw new IdNotFoundException("No Active Bidder with the given ID");
        }

        public override async Task<IEnumerable<Bidder>> GetAll()
        {
            var bidders = _auctionContext.Bidders.Where(b => b.User.Status == "Active").Include(b=>b.User);
            if (bidders.Count() == 0)
                throw new CollectionEmptyException("No Active Bidders in the database");
            return await bidders.ToListAsync();
        }
    }
}