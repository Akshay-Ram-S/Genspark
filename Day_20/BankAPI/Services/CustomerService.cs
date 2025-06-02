using BankAPI.Interfaces;
using BankAPI.Misc;
using BankAPI.Models;
using BankAPI.Models.Dtos;
using BankAPI.Repositories;
using System.Threading.Tasks;

namespace BankAPI.Services
{
    public class CustomerService : ICustomerService
    {
        CustomerMapper customerMapper;
        private readonly CustomerRepository _customerRepository;

        public CustomerService(CustomerRepository customerRepository)
        {
            customerMapper = new();
            _customerRepository = customerRepository;
        }

        public async Task<Customer> CreateCustomer(CustomerAddRequestDto customer)
        {
            try
            {
                var newCustomer = customerMapper.MapCustomerAddRequest(customer);
                newCustomer = await _customerRepository.Add(newCustomer);
                if (newCustomer == null)
                    throw new Exception("Could not add customer");

                return newCustomer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public async Task<Customer> FindCustomerById(int id)
        {
            try
            {
                if (id < 1)
                {
                    throw new Exception("Please Enter a positive customer ID");
                }
                var customer = await _customerRepository.Get(id);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {id} not found.");
                }

                return customer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAll();
            if (customers.Count() == 0)
            {
                throw new Exception("No Customers found in database");
            }
            return customers;
        }
    }
}
