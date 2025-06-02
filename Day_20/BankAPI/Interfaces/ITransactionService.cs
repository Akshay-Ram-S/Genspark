
using BankAPI.Models;

namespace BankAPI.Interfaces
{
    public interface ITransactionService
    {
        public Task<Transaction> Deposit(string accountNumber, double amount);
        public Task<Transaction> Withdraw(string accountNumber, double amount);
    }
}
