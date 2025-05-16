using DatabaseLayer.DTOs;
using DatabaseLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Transactions
{
    public interface ISuspiciousTransactionService
    {
        Task<List<SuspiciousTransactionDto>> DetectSuspiciousTransactionsAsync();
    }
}
