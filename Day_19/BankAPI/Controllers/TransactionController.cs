using BankAPI.Interfaces;
using BankAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromQuery] string accountNumber, [FromQuery] double amount)
        {
            var transaction = await _transactionService.Deposit(accountNumber, amount);
            return Ok(transaction);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromQuery] string accountNumber, [FromQuery] double amount)
        {
            var transaction = await _transactionService.Withdraw(accountNumber, amount);
            return Ok(transaction);
        }
    }
}
