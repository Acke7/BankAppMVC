using BankAppMVC.Models.ViewModels.AccountVm;
using DatabaseLayer.DTOs;
using DatabaseLayer.DTOs.Account;
using DatabaseLayer.DTOs.Transaktion;
using DatabaseLayer.Models;
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
        public async Task<IActionResult> Details(int AccountId)
        {
            var customerId = await _accountService.GetCustomerIdByAccountIdAsync(AccountId);
            if (customerId == null)
                return BadRequest("No customer linked to this account.");

            var transactions = await _accountService.GetTransactionsByAccountNumber(AccountId);
            var accountBalance = await _accountService.GetBalanceByAccountId(AccountId);

            var viewModel = transactions.Select(t => new AccountTransactionsViewModel
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
                AccountId = AccountId
            }).ToList();

            ViewBag.AccountId = AccountId;
            ViewBag.CustomerId = customerId;

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Deposit(int AccountId)
        {
            return View(new DepositViewModel());
        }
        public IActionResult Withdraw(int AccountId)
        {
            return View(new WithdrawViewModel());
        }
        public IActionResult Transfer(int AccountId)
        {
            return View(new TransferViewModel { FromAccountId = AccountId });
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositViewModel viewModel, int AccountId)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel); // return with errors
            }

            var vm = new TransactionDto
            {
                AccountId = AccountId,
                Amount = viewModel.Amount,
                Operation = viewModel.Operation,
                Symbol = viewModel.Symbol
            };

            var result = await _transactionService.DepositAsync(vm);

            if (result != ErrorCode.OK)
            {
                ModelState.AddModelError("", GetErrorMessage(result));
                return View(viewModel);
            }
            TempData["SuccessMessage"] = "Deposit completed successfully!";
            return RedirectToAction("Details", "Account", new { AccountId = viewModel.AccountId });
        }
        [HttpPost]
        public async Task<IActionResult> Withdraw(WithdrawViewModel viewModel, int AccountId)
        {
            var dto = new TransactionDto
            {
                AccountId = AccountId,
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
            return HandleResult(result, viewModel.AccountId);
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferViewModel viewModel, int AccountId)
        {
            var dto = new TransferDto
            {
                FromAccountId = AccountId,
                ToAccountId = viewModel.ToAccountId,
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
            return HandleResult(result, AccountId);
        }

        private IActionResult HandleResult(ErrorCode result, int AccountId)
        {
            if (result != ErrorCode.OK)
            {
                ModelState.AddModelError("", GetErrorMessage(result));
                return View(); // or return the same View with model
            }

            // ✅ Set success message before redirecting

            TempData["SuccessMessage"] = "Transfer completed successfully!";
            return RedirectToAction("Details", "Account", new { AccountId });
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
        public async Task<IActionResult> Edit(int AccountId, int customerID)
        {
            var account = await _accountService.GetAccountByAccountNumber(AccountId);
            if (account == null)
                return NotFound();

            var model = new UpdateAccountViewModel
            {
                CustomerId = customerID,
                AccountId = account.AccountId,
                Frequency = account.Frequency,
                Balance = account.Balance,
                AccountNumber = account.AccountNumber
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

            await _accountService.UpdateAccount(model.AccountId, dto);
            TempData["ToastMessage"] = "Account successfully Edited!";
            TempData["ToastType"] = "warning"; // use 'success', 'warning', or 'danger'

            return RedirectToAction("Details", "Customer", new { id = model.CustomerId });
        }

        // POST: /Account/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int AccountId, int customerId)
        {
            await _accountService.DeleteAccount(AccountId);
            TempData["ToastMessage"] = "Account successfully 'Deleted'!";
            TempData["ToastType"] = "danger"; // use 'success', 'warning', or 'danger'
            return RedirectToAction("Details", "Customer", new { id = customerId });
        }

        public async Task<IActionResult> LoadMoreTransactions(int AccountId, int skip)
        {

            var transactions = await _accountService.GetTransactionsByAccountNumber(AccountId);
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

