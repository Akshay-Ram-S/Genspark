using System.Linq;
using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public  class AuditRepository : Repository<Guid, Audit>
    {
        public AuditRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<Audit> Get(Guid key)
        {
            var audit = await _auctionContext.Audits.Where(a => a.Id == key).SingleOrDefaultAsync();
            if (audit == null)
                throw new IdNotFoundException("No Audit with the given ID");

            return audit ?? throw new IdNotFoundException("No Audit with the given ID");
        }

        public override async Task<IEnumerable<Audit>> GetAll()
        {
            var audits = _auctionContext.Audits.ToListAsync();
            if (audits == null)
                throw new CollectionEmptyException("No Audits in the database");
            return await audits;
        }
    }
}