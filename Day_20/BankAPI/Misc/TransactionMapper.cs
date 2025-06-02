using BankAPI.Models;
using BankAPI.Models.Dtos;

namespace BankAPI.Misc
{
    public class TransactionMapper
    {
        public Transaction? MapTransactionAddRequest(string AccountNumber, double Amount, string ActionType)
        {
            Transaction transaction = new();
            transaction.AccountNumber = AccountNumber;
            transaction.Amount = Amount;
            transaction.ActionType = ActionType;
            transaction.Timestamp = DateTime.UtcNow;
            return transaction;
        }
    }
}