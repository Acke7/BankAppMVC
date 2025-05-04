using BankAppMVC.Models.ViewModels.AccountVm;
using DatabaseLayer.DTOs;
using DatabaseLayer.DTOs.Account;
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
        public async Task<IActionResult> Deposit(DepositViewModel viewModel, int AccountNumber)
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
                Symbol = viewModel.Symbol
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
                Symbol = viewModel.Symbol

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

        // GET: /Account/Create?customerId=5
        public IActionResult Create(int customerId)
        {
            var model = new CreateAccountViewModel { CustomerId = customerId };
            return View(model);
        }

        // POST: /Account/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CreateAccountDto
            {
                CustomerId = model.CustomerId,
                Frequency = model.Frequency,
                Balance = model.Balance
            };

            await _accountService.CreateAccount(dto);
            TempData["ToastMessage"] = "Account successfully Created!";
            TempData["ToastType"] = "success"; // use 'success', 'warning', or 'danger'

            return RedirectToAction("Details", "Customer", new { id = model.CustomerId });
        }

        // GET: /Account/Edit/5
        public async Task<IActionResult> Edit(int AccountNumber, int customerID)
        {
            var account = await _accountService.GetAccountByAccountNumber(AccountNumber);
            if (account == null)
                return NotFound();

            var model = new UpdateAccountViewModel
            {
                CustomerId = customerID,
                AccountId = account.AccountId,
                Frequency = account.Frequency,
                Balance = account.Balance,
                AccountNumber = AccountNumber
            };

            return View(model);
        }

        // POST: /Account/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new UpdateAccountDto
            {
                AccountId = model.AccountId,
                Frequency = model.Frequency,
                Balance = model.Balance,
                AccountNumber = model.AccountNumber
            };

            await _accountService.UpdateAccount(model.AccountNumber, dto);
            TempData["ToastMessage"] = "Account successfully Edited!";
            TempData["ToastType"] = "warning"; // use 'success', 'warning', or 'danger'

            return RedirectToAction("Details", "Customer", new { id = model.CustomerId });
        }

        // POST: /Account/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int AccountNumber, int customerId)
        {
            await _accountService.DeleteAccount(AccountNumber);
            TempData["ToastMessage"] = "Account successfully 'Deleted'!";
            TempData["ToastType"] = "danger"; // use 'success', 'warning', or 'danger'
            return RedirectToAction("Details", "Customer", new { id = customerId });
        }

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
    }

}

