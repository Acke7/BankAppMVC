using BankAppMVC.Models.ViewModels;
using DatabaseLayer.DTOs;
using DatabaseLayer.DTOs.Transaktion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Services.Account;
using Services.Customers;
using Services.Transactions;

namespace BankAppMVC.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        public AccountController(IAccountService accountService, ITransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
        }

        //public async Task<IActionResult> GetBalance(int accountNumber)
        //{
        //    var balance = await _accountService.GetBalanceByAccountId(accountNumber);
        //    var vm = new AccountViewModel()
        //    {
        //        Balance = balance
        //    };
        //    return View(vm);
        //}

        //[HttpGet]
        public async Task<IActionResult> LoadMoreTransactions(int AccountNumber, int skip)
        {
           
            var transactions = await _accountService.GetTransactionsByAccountNumber(AccountNumber);
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
        public async Task<IActionResult> Details(int AccountNumber)
        {
            var transactions = await _accountService.GetTransactionsByAccountNumber(AccountNumber);
            var accountBalance = await _accountService.GetBalanceByAccountId(AccountNumber);
         
            var viewModel = transactions
                 
                /*.ThenByDescending( t => transactions.)*/ // Order transactions by date from newest
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
                    AccountNumber = AccountNumber
                }).ToList();

            ViewBag.AccountNumber = AccountNumber;
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Deposit(int AccountNumber)
        {
            return View(new DepositViewModel());
        }
        public IActionResult Withdraw(int AccountNumber)
        {
            return View(new WithdrawViewModel());
        }
        public IActionResult Transfer(int AccountNumber)
        {
            return View(new TransferViewModel { FromAccountNumber = AccountNumber });
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositViewModel viewModel ,int AccountNumber)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel); // return with errors
            }

            var dto = new TransactionDto
            {
                AccountNumber = AccountNumber,
                Amount = viewModel.Amount,
                Operation = viewModel.Operation,
               Symbol=viewModel.Symbol
            };

            var result = await _transactionService.DepositAsync(dto);

            if (result != ErrorCode.OK)
            {
                ModelState.AddModelError("", GetErrorMessage(result));
                return View(viewModel);
            }
            TempData["SuccessMessage"] = "Deposit completed successfully!";
            return RedirectToAction("Details", "Account", new { accountNumber = viewModel.AccountNumber });
        }
        [HttpPost]
        public async Task<IActionResult> Withdraw(WithdrawViewModel viewModel, int AccountNumber)
        {
            var dto = new TransactionDto
            {
                AccountNumber = AccountNumber,
                Amount = viewModel.Amount,
                Operation = viewModel.Operation,
                   Symbol = viewModel.Symbol
            };

            var result = await _transactionService.WithdrawAsync(dto);
            if (result != ErrorCode.OK)
            {
                ModelState.AddModelError("", GetErrorMessage(result));
                return View(viewModel);
            }
            TempData["SuccessMessage"] = "Withdraw completed successfully!";
            return HandleResult(result, viewModel.AccountNumber);
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferViewModel viewModel, int AccountNumber)
        {
            var dto = new TransferDto
            {
                FromAccountNumber = AccountNumber,
                ToAccountNumber = viewModel.ToAccountNumber,
                Amount = viewModel.Amount,
                Operation = viewModel.Operation,
                Symbol=viewModel.Symbol

            };

            var result = await _transactionService.TransferAsync(dto);
            if (result != ErrorCode.OK)
            {
                ModelState.AddModelError("", GetErrorMessage(result));
                return View(viewModel);
            }
            TempData["SuccessMessage"] = "Transfer completed successfully!";
            return HandleResult(result, AccountNumber);
        }

        private IActionResult HandleResult(ErrorCode result, int accountNumber)
        {
            if (result != ErrorCode.OK)
            {
                ModelState.AddModelError("", GetErrorMessage(result));
                return View(); // or return the same View with model
            }

            // ✅ Set success message before redirecting

            TempData["SuccessMessage"] = "Transfer completed successfully!";
            return RedirectToAction("Details", "Account", new { accountNumber });
        }

        private string GetErrorMessage(ErrorCode code)
        {
            return code switch
            {
                ErrorCode.BalanceTooLow => "Balance is too low.",
                ErrorCode.IncorrectAmount => "Amount must be greater than zero.",
                ErrorCode.AccountNotFound => "Invalid input or account.",
                _ => "Unknown error"
            };
        }
    }
}
