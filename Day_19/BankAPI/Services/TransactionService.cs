using BankAPI.Interfaces;
using BankAPI.Models;
using BankAPI.Repositories;
using BankAPI.Misc;
using System;
using System.Threading.Tasks;

namespace BankAPI.Services
{
    public class TransactionService : ITransactionService
    {
        TransactionMapper transactionMapper;
        private readonly TransactionRepository _transactionRepository;
        private readonly AccountRepository _accountRepository;

        public TransactionService(TransactionRepository transactionRepository,
                                  AccountRepository accountRepository)
        {
            transactionMapper = new();
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Transaction> Deposit(string accountNumber, double amount)
        {
            var account = await _accountRepository.Get(accountNumber);
            if (account == null)
                throw new Exception("Account not found");

            account.Balance += (double)amount;
            await _accountRepository.Update(accountNumber, account);

            var transaction = transactionMapper.MapTransactionAddRequest(accountNumber, amount, "Deposit");

            return await _transactionRepository.Add(transaction);
        }

        public async Task<Transaction> Withdraw(string accountNumber, double amount)
        {
            var account = await _accountRepository.Get(accountNumber);
            if (account == null)
                throw new Exception("Account not found");

            if (account.Balance < (double)amount)
                throw new Exception("Insufficient Balance");

            account.Balance -= (double)amount;
            await _accountRepository.Update(accountNumber, account);

            var transaction = transactionMapper.MapTransactionAddRequest(accountNumber, amount, "Withdrawal");

            return await _transactionRepository.Add(transaction);
        }
    }
}
