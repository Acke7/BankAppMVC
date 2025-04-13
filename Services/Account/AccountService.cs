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
                    // Return empty list or throw an error
                    return new List<AccountTransaktionDto>();
                }

                // 2. Now get transactions by AccountId
                var transactions = await _context.Transactions
                    .Where(t => t.AccountId == account.AccountId)
                    .Select(t => new AccountTransaktionDto
                    {
                        Date = DateOnly.FromDateTime(t.Date.ToDateTime(TimeOnly.MinValue)),
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


        }
    }



