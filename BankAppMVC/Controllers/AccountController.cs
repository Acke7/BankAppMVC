using BankAppMVC.Models.ViewModels;
using DatabaseLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Services.Account;
using Services.Customers;

namespace BankAppMVC.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
         
        public async Task<IActionResult> GetBalance(int accountId)
        {
            var balance = await _accountService.GetBalanceByAccountId(accountId);
            var vm = new AccountViewModel()
            {
                Balance = balance
            };
            return View(vm);
        }

        //[HttpGet]
        public async Task<IActionResult> LoadMoreTransactions(int accountId, int skip)
        {
           
            var transactions = await _accountService.GetTransactionByAccountId(accountId);
            var next20 = transactions.Skip(skip).Take(20).Select(t => new AccountTransactionsViewModel
            {
                
                Date = t.Date,
                Type = t.Type,
                Operation = t.Operation,
                Amount = t.Amount,
                Balance = t.Balance,
                Symbol = t.Symbol,
                Bank = t.Bank,
                Account = t.Account

            }).ToList();

            return PartialView("_TransactionRows", next20);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int accountId)
        {
            var transactions = await _accountService.GetTransactionByAccountId(accountId);
            var accountBalance = await _accountService.GetBalanceByAccountId(accountId);
            var viewModel = transactions
                .OrderByDescending(t => t.Date) // Order transactions by date from newest
                .Select(t => new AccountTransactionsViewModel
                {
                    AccountBalance = accountBalance,
                    Date = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Symbol = t.Symbol,
                    Bank = t.Bank,
                    Account = t.Account,
                    AccountNumber = t.AccountNumber
                }).ToList();

            ViewBag.AccountId = accountId;
            return View(viewModel);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
