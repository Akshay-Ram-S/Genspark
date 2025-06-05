using DocSharingAPI.Contexts;
using DocSharingAPI.Interfaces;
using DocSharingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DocSharingAPI.Repositories
{
    public  class UserRepository : Repository<string, User>
    {
        public UserRepository(DocumentContext documentContext) : base(documentContext)
        {
        }

        public override async Task<User> Get(string key)
        {
            var user = await _documentContext.Users.SingleOrDefaultAsync(p => p.Email == key);

            return user??throw new Exception("No User with the given ID");
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var users = _documentContext.Users;
            if (users.Count() == 0)
                throw new Exception("No User in the database");
            return (await users.ToListAsync());
        }
    }
}