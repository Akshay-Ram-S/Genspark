using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public class RefreshTokenRepository : Repository<string, RefreshToken>
    {
        public RefreshTokenRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<RefreshToken> Get(string token)
        {
            var refreshToken = await _auctionContext.RefreshTokens.Where(rt=>rt.IsRevoked == false)
                                                                    .FirstOrDefaultAsync(rt => rt.Token == token);

            return refreshToken ?? throw new IdNotFoundException("No Token found.");
        }

        public override async Task<IEnumerable<RefreshToken>> GetAll()
        {
            var tokens = await _auctionContext.RefreshTokens.Where(rt=>rt.IsRevoked == false).ToListAsync();
            if (tokens.Count() == 0)
                throw new CollectionEmptyException("No Items in the database");
            return tokens;
        }

    }
}