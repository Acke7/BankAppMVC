using DatabaseLayer.DTOs.Account;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Services.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly BankAppDataContext _context;

        public TransactionService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task DepositAsync(int accountId, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) throw new Exception("Account not found");

            var newBalance = account.Balance + amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                Type = "Credit",
                Operation = "Deposit",
                Date = DateTime.Now,
                Balance = newBalance
            };

            account.Balance = newBalance;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task WithdrawAsync(int accountId, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) throw new Exception("Account not found");
            if (account.Balance < amount) throw new Exception("Insufficient funds");

            var newBalance = account.Balance - amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                Type = "Debit",
                Operation = "Withdraw",
                Date = DateTime.Now,
                Balance = newBalance
            };

            account.Balance = newBalance;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            if (fromAccountId == toAccountId)
                throw new Exception("Cannot transfer to the same account");

            var from = await _context.Accounts.FindAsync(fromAccountId);
            var to = await _context.Accounts.FindAsync(toAccountId);

            if (from == null || to == null) throw new Exception("One of the accounts does not exist");
            if (from.Balance < amount) throw new Exception("Insufficient funds");

            var fromNewBalance = from.Balance - amount;
            var toNewBalance = to.Balance + amount;

            var outTransaction = new Transaction
            {
                AccountId = fromAccountId,
                Amount = amount,
                Type = "Debit",
                Operation = "Transfer Out",
                Date = DateTime.Now,
                Balance = fromNewBalance,
                Account = toAccountId.ToString()
            };

            var inTransaction = new Transaction
            {
                AccountId = toAccountId,
                Amount = amount,
                Type = "Credit",
                Operation = "Transfer In",
                Date = DateTime.Now,
                Balance = toNewBalance,
                Account = fromAccountId.ToString()
            };

            from.Balance = fromNewBalance;
            to.Balance = toNewBalance;

            _context.Transactions.AddRange(outTransaction, inTransaction);
            await _context.SaveChangesAsync();
        }
    }

}
