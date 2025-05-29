using BankAPI.Interfaces;
using BankAPI.Models;
using BankAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerAddRequestDto customer)
        {
            var createdCustomer = await _customerService.CreateCustomer(customer);
            return Created("", createdCustomer);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(int customerId)
        {
            var account = await _customerService.FindCustomerById(customerId);
            if (account == null)
                return NotFound();
            return Ok(account);
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomer()
        {
            var account = await _customerService.GetAllCustomers();
            if (account == null)
                return NotFound();
            return Ok(account);
        }
    }
}
