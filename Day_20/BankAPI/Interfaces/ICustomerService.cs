
using BankAPI.Models;
using BankAPI.Models.Dtos;

namespace BankAPI.Interfaces
{
    public interface ICustomerService
    {
        public Task<Customer> CreateCustomer(CustomerAddRequestDto customerDto);
        public Task<Customer> FindCustomerById(int id);
        public Task<IEnumerable<Customer>> GetAllCustomers();
    }
}
