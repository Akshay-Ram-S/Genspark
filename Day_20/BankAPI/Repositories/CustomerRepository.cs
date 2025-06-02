
using BankAPI.Contexts;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Repositories
{
    public  class CustomerRepository : Repository<int, Customer>
    {
        public CustomerRepository(BankContext bankContext) : base(bankContext)
        {
        }

        public override async Task<Customer> Get(int key)
        {
            var customer = await _bankContext.Customers.SingleOrDefaultAsync(d => d.Id == key);

            return customer??throw new Exception("No customer with the given ID");
        }

        public override async Task<IEnumerable<Customer>> GetAll()
        {
            var customers = _bankContext.Customers;
            if (customers.Count() == 0)
                throw new Exception("No customers in the database");

            return await customers.ToListAsync();
        }
    }
}
