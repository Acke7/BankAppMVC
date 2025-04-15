using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Transactions
{
    public interface ITransactionService
    {
        Task DepositAsync(int accountNumber, decimal amount);
        Task WithdrawAsync(int accountNumber, decimal amount);
        Task TransferAsync(int fromAccountNumber, int toAccountNumber, decimal amount);
    }
}
