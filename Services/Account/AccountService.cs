using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Account
{
  using DatabaseLayer.DTOs;
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

        public async Task<List<AccountTransaktionDto>> GetTransactionByAccountId(int accountId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .Select(t => new AccountTransaktionDto
                {
                    AccountNumber=t.AccountId,
                    Date = DateOnly.FromDateTime(t.Date.ToDateTime(TimeOnly.MinValue)), // Convert DateOnly to DateTime
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
        }
           


        }

}
