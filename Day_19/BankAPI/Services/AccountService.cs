using BankAPI.Interfaces;
using BankAPI.Misc;
using BankAPI.Models;
using BankAPI.Models.Dtos;
using BankAPI.Repositories;
using System;
using System.Threading.Tasks;

namespace BankAPI.Services
{
    public class AccountService : IAccountService
    {
        AccountMapper accountMapper;
        private readonly AccountRepository _accountRepository;

        public AccountService(AccountRepository accountRepository)
        {
            accountMapper = new();
            _accountRepository = accountRepository;
        }

        public async Task<Account> CreateAccount(AccountAddRequestDto customer)
        {
            try
            {
                var newAccount = accountMapper.MapAccountAddRequest(customer);
                newAccount = await _accountRepository.Add(newAccount);
                if (newAccount == null)
                    throw new Exception("Could not add customer");

                return newAccount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public async Task<Account> GetAccount(string accountNumber)
        {
            try
            {
                var account = await _accountRepository.Get(accountNumber);
                if (account == null)
                {
                    throw new KeyNotFoundException($"Account with ID {accountNumber} not found.");
                }

                return account;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<Account>> GetAllAccounts()
        {
            var accounts = await _accountRepository.GetAll();
            if (accounts.Count() == 0)
            {
                throw new Exception("No Customers found in database");
            }
            return accounts;
        }

    }
}
