
using BankAPI.Models;
using BankAPI.Models.Dtos;

namespace BankAPI.Interfaces
{
    public interface IAccountService
    {
        public Task<Account> CreateAccount(AccountAddRequestDto account);
        public Task<Account> GetAccount(string accountNumber);
        public Task<IEnumerable<Account>> GetAllAccounts();
    }
}
