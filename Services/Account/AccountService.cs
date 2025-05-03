using DatabaseLayer.DTOs.Account;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Account
    {
        public class AccountService : IAccountService
        {
            private readonly BankAppDataContext _context;

            public AccountService(BankAppDataContext context)
            {
                _context = context;
            }



        public async Task<List<AccountTransaktionDto>> GetTransactionsByAccountNumber(int accountNumber)
        {
            // 1. Try to find the account
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                return new List<AccountTransaktionDto>();
            }

            // 2. Get and sort transactions before projecting to DTO
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == account.AccountId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId) // use TransactionId only here
                .Select(t => new AccountTransaktionDto
                {
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Symbol = t.Symbol,
                    Bank = t.Bank,
                    Account = t.Account
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<decimal> GetBalanceByAccountId(int accountNumber)
            {
                var balance = await _context.Accounts
                    .Where(a => a.AccountNumber == accountNumber)
                    .Select(a => a.Balance)
                    .FirstOrDefaultAsync();

                return balance;
            }

        public async Task<AccountDTO> GetAccountByAccountNumber(int accountNumber)
        {
            var account = await _context.Accounts
               .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null) return null;

            return new AccountDTO
            {
                AccountId = account.AccountId,
                Frequency = account.Frequency,
                Balance = account.Balance,
                IsActive = account.IsActive,
                AccountNumber = account.AccountNumber
            };
        }
        public async Task CreateAccount(CreateAccountDto dto)
        {
            var newAccount = new DatabaseLayer.Models.Account
            {
                Frequency = dto.Frequency,
                Balance = dto.Balance,
                Created = DateOnly.FromDateTime(DateTime.UtcNow),
                AccountNumber = new Random().Next(10000000, 99999999), // generate random number (better generate safer)
                IsActive = true
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // Now create Disposition to link Account to Customer
            var newDisposition = new Disposition
            {
                AccountId = newAccount.AccountId,
                CustomerId = dto.CustomerId,
                Type = "OWNER"
            };
            _context.Dispositions.Add(newDisposition);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccount(int accountNumber, UpdateAccountDto dto)
        {
            var account = await _context.Accounts
               .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null) return;

            account.Frequency = dto.Frequency;
          

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccount(int accountNumber)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null) return;

            // Mark account as inactive instead of deleting
            account.IsActive = false;

            // Update the account in the database
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }



}
    


