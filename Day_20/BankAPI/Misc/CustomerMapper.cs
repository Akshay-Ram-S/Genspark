using BankAPI.Models;
using BankAPI.Models.Dtos;

namespace BankAPI.Misc
{
    public class CustomerMapper
    {
        public Customer? MapCustomerAddRequest(CustomerAddRequestDto addRequestDto)
        {
            Customer customer = new();
            customer.Name = addRequestDto.Name;
            customer.Dob = addRequestDto.Dob;
            customer.Pan = addRequestDto.Pan;
            customer.Email = addRequestDto.Email;
            customer.Status = "Active";
            return customer;
        }
    }
}