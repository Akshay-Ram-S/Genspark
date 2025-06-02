
using BankAPI.Contexts;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Repositories
{
    public  class TransactionRepository : Repository<int, Transaction>
    {
        public TransactionRepository(BankContext bankContext) : base(bankContext)
        {
        }

        public override async Task<Transaction> Get(int id)
        {
            var transaction = await _bankContext.Transactions.SingleOrDefaultAsync(d => d.Id == id);

            return transaction??throw new Exception("No transaction with the given ID");
        }

        public override async Task<IEnumerable<Transaction>> GetAll()
        {
            var transactions = _bankContext.Transactions;
            if (transactions.Count() == 0)
                throw new Exception("No transactions in the database");

            return await transactions.ToListAsync();
        }
    }
}
