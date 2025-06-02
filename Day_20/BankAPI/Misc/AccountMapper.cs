using BankAPI.Models;
using BankAPI.Models.Dtos;

namespace BankAPI.Misc
{
    public class AccountMapper
    {
        public Account? MapAccountAddRequest(AccountAddRequestDto addRequestDto)
        {
            Account account = new();
            account.AccountNumber = addRequestDto.AccountNumber;
            account.CustomerId = addRequestDto.CustomerId;
            account.Balance = addRequestDto.Balance;
            account.AccountType = addRequestDto.AccountType;
            account.Status = "Active";
            return account;
        }
    }
}