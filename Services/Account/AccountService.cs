using DatabaseLayer.DTOs.Account;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer.Models;
using Microsoft.Identity.Client;

namespace Services.Account
    {
        public class AccountService : IAccountService
        {
            private readonly BankAppDataContext _context;

            public AccountService(BankAppDataContext context)
            {
                _context = context;
            }

        public async Task<List<DatabaseLayer.Models.Account>> GetAccountsByCustomerIdAsync(int customerId)
        {
            return await _context.Accounts
                .Where(a => a.Dispositions.Any(d => d.CustomerId == customerId))
                .ToListAsync();
        }

        public async Task<List<AccountTransaktionDto>> GetTransactionsByAccountNumber(int accountId)
        {
            // 1. Try to find the account
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

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

        public async Task<decimal> GetBalanceByAccountId(int accountId)
            {
                var balance = await _context.Accounts
                    .Where(a => a.AccountId == accountId)
                    .Select(a => a.Balance)
                    .FirstOrDefaultAsync();

                return balance;
            }

        public async Task<AccountDTO> GetAccountByAccountNumber(int accountId)
        {
            var account = await _context.Accounts
               .FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null) return null;

            return new AccountDTO
            {
                AccountId = account.AccountId,
                Frequency = account.Frequency,
                Balance = account.Balance,
                IsActive = account.IsActive,
             
            };
        }
        public async Task CreateAccount(CreateAccountDto dto)
        {
            var newAccount = new DatabaseLayer.Models.Account
            {
                Frequency = dto.Frequency,
                Balance = dto.Balance,
                Created = DateOnly.FromDateTime(DateTime.UtcNow),
              
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

        public async Task UpdateAccount(int accountId, UpdateAccountDto dto)
        {
            var account = await _context.Accounts
               .FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null) return;

            account.Frequency = dto.Frequency;
          

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccount(int accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null) return;

            // Mark account as inactive instead of deleting
            account.IsActive = false;

            // Update the account in the database
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }



}
    


