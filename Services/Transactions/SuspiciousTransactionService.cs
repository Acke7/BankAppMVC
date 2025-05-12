using DatabaseLayer.DTOs;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Services.Transactions;

public class SuspiciousTransactionService : ISuspiciousTransactionService
{
    private readonly BankAppDataContext _context;

    public SuspiciousTransactionService(BankAppDataContext context)
    {
        _context = context;
    }

    public async Task<List<SuspiciousTransactionDto>> DetectSuspiciousTransactionsAsync()
    {
        var suspicious = new List<SuspiciousTransactionDto>();
        var now = DateTime.Now;

        // 1. Load existing suspicious transaction IDs once
        var alreadyStoredIds = await _context.SuspiciousTransactions
            .AsNoTracking()
            .Select(s => s.TransactionId)
            .ToHashSetAsync();

        // 2. Load customers, accounts, transactions
        var customers = await _context.Customers
            .AsNoTracking()
            .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                    .ThenInclude(a => a.Transactions)
            .AsSplitQuery()
            .ToListAsync();

        foreach (var customer in customers)
        {
            foreach (var disposition in customer.Dispositions)
            {
                var account = disposition.Account;
                if (account == null) continue;

                var transactions = account.Transactions
                    .Where(t => t.Amount > 0) // Optional: skip 0-value tx
                    .OrderBy(t => t.Date)
                    .ToList();

                foreach (var tx in transactions)
                {
                    if (alreadyStoredIds.Contains(tx.TransactionId)) continue;

                    var txDate = tx.Date.ToDateTime(TimeOnly.MinValue);

                    // Rule 1
                    if (tx.Amount > 15000)
                    {
                        suspicious.Add(new SuspiciousTransactionDto
                        {
                            Amount=tx.Amount,
                            TransactionId = tx.TransactionId,
                            CustomerId = customer.CustomerId,
                            Country = customer.Country,
                            DetectedAt = now,
                            AccountId = account.AccountId,
                            FullName = $"{customer.Givenname} {customer.Surname}" // 👈 adjust based on your Customer model
                        });
                        continue;
                    }

                    // Rule 2
                    var windowStart = txDate.AddHours(-72);
                    var sum = transactions
                        .Where(t =>
                        {
                            var tDate = t.Date.ToDateTime(TimeOnly.MinValue);
                            return tDate >= windowStart && tDate <= txDate;
                        })
                        .Sum(t => t.Amount);

                    if (sum > 23000)
                    {
                        suspicious.Add(new SuspiciousTransactionDto
                        {
                            Amount = tx.Amount,
                            TransactionId = tx.TransactionId,
                            CustomerId = customer.CustomerId,
                            Country = customer.Country,
                            DetectedAt = now,
                            AccountId = account.AccountId,
                            FullName = $"{customer.Givenname} {customer.Surname}" // 👈 adjust based on your Customer model
                        });
                    }
                }
            }
        }

        // 3. Save new suspicious transactions
        var entities = suspicious.Select(s => new SuspiciousTransaction
        {
            TransactionId = s.TransactionId,
            CustomerId = s.CustomerId,
            Country = s.Country,
            DetectedAt = s.DetectedAt
        });

        await _context.SuspiciousTransactions.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        return suspicious;
    }

}
