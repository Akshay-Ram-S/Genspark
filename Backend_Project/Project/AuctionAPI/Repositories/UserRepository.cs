using AuctionAPI.Contexts;
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuctionAPI.Repositories
{
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(AuctionContext auctionContext) : base(auctionContext)
        {
        }

        public override async Task<User> Get(string key)
        {
            var user = await _auctionContext.Users.Where(u => u.Status == "Active").SingleOrDefaultAsync(u => u.Email == key);

            return user??throw new IdNotFoundException("No User with the given ID");
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var users = _auctionContext.Users.Where(u => u.Status == "Active");

            if (users == null)
                throw new CollectionEmptyException("No active users in the database");

            return await users.ToListAsync();
        }

    }
}