using BankAPI.Interfaces;
using BankAPI.Models;
using BankAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountAddRequestDto account)
        {
            var createdAccount = await _accountService.CreateAccount(account);
            return CreatedAtAction(nameof(GetAccount), new { accountId = createdAccount.Id }, createdAccount);
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccount(string accountNumber)
        {
            var account = await _accountService.GetAccount(accountNumber);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var account = await _accountService.GetAllAccounts();
            if (account == null)
                return NotFound();
            return Ok(account);
        }

    }
}
