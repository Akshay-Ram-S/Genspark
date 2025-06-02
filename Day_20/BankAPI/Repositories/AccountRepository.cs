using BankAPI.Contexts;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Repositories
{
    public  class AccountRepository : Repository<string, Account>
    {
        public AccountRepository(BankContext bankContext) : base(bankContext)
        {
        }

        public override async Task<Account> Get(string key)
        {
            var account = await _bankContext.Accounts.SingleOrDefaultAsync(d => d.AccountNumber == key);

            return account??throw new Exception("No account with the given ID");
        }

        public override async Task<IEnumerable<Account>> GetAll()
        {
            var accounts = _bankContext.Accounts;
            if (accounts.Count() == 0)
                throw new Exception("No accounts in the database");

            return await accounts.ToListAsync();
        }
    }
}
