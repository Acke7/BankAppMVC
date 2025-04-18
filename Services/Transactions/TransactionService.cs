using DatabaseLayer.DTOs.Transaktion;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Services.Transactions;
using System;

public class TransactionService : ITransactionService
{
    private readonly BankAppDataContext _context;

    public TransactionService(BankAppDataContext context)
    {
        _context = context;
    }

    public async Task<ErrorCode> DepositAsync(TransactionDto dto)
    {
        if (dto.Amount <= 0)
            return ErrorCode.IncorrectAmount;

        var account =  _context.Accounts
            .FirstOrDefault(a => a.AccountNumber == dto.AccountNumber);

        if (account == null)
            return ErrorCode.AccountNotFound;

        account.Balance += dto.Amount;

        var transaction = new Transaction
        {
            AccountId = account.AccountId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Type = "Credit",
            Operation = dto.Operation,
            Amount = dto.Amount,
            Balance = account.Balance,
            Symbol = dto.Symbol,
            Bank = dto.Bank,
            Account = dto.Account
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return ErrorCode.OK;
    }

    public async Task<ErrorCode> WithdrawAsync(TransactionDto dto)
    {
        if (dto.Amount <= 0)
            return ErrorCode.IncorrectAmount;

        var account = _context.Accounts
            .FirstOrDefault(a => a.AccountNumber == dto.AccountNumber);

        if (account == null)
            return ErrorCode.AccountNotFound;

        if (account.Balance < dto.Amount)
            return ErrorCode.BalanceTooLow;

        account.Balance -= dto.Amount;

        var transaction = new Transaction
        {
            AccountId = account.AccountId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Type = "Debit",
            Operation = dto.Operation,
            Amount = dto.Amount,
            Balance = account.Balance,
            Symbol = dto.Symbol,
            Bank = dto.Bank,
            Account = dto.Account
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return ErrorCode.OK;
    }

    public async Task<ErrorCode> TransferAsync(TransferDto dto)
    {
        if (dto.Amount <= 0)
            return ErrorCode.IncorrectAmount;

        if (dto.FromAccountNumber == dto.ToAccountNumber)
            return ErrorCode.YouCantTransferToSameAccount;

        var fromAccount = _context.Accounts
            .FirstOrDefault(a => a.AccountNumber == dto.FromAccountNumber);

        var toAccount =  _context.Accounts
            .FirstOrDefault(a => a.AccountNumber == dto.ToAccountNumber);



        if (fromAccount == null || toAccount == null)
            return ErrorCode.AccountNotFound;
    

        if (fromAccount.Balance < dto.Amount)
            return ErrorCode.BalanceTooLow;

        fromAccount.Balance -= dto.Amount;
        toAccount.Balance += dto.Amount;

        var now = DateOnly.FromDateTime(DateTime.Now);

        var debit = new Transaction
        {
            AccountId = fromAccount.AccountId,
            Date = now,
            Type = "Debit",
            Operation = dto.Operation,
            Amount = dto.Amount,
            Balance = fromAccount.Balance,
            Symbol = dto.Symbol,
            Bank = dto.Bank,
            Account = toAccount.AccountNumber.ToString()
        };

        var credit = new Transaction
        {
            AccountId = toAccount.AccountId,
            Date = now,
            Type = "Credit",
            Operation = dto.Operation,
            Amount = dto.Amount,
            Balance = toAccount.Balance,
            Symbol = dto.Symbol,
            Bank = dto.Bank,
            Account = fromAccount.AccountNumber.ToString()
        };

        _context.Transactions.AddRange(debit, credit);
        await _context.SaveChangesAsync();

        return ErrorCode.OK;
    }
}
