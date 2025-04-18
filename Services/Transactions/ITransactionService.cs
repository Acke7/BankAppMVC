using DatabaseLayer.DTOs.Transaktion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum ErrorCode
{
    OK,
    BalanceTooLow,
    IncorrectAmount,
    AccountNotFound,
    YouCantTransferToSameAccount
}
namespace Services.Transactions
{
    public interface ITransactionService
    {

        Task<ErrorCode> DepositAsync(TransactionDto dto);
        Task<ErrorCode> WithdrawAsync(TransactionDto dto);
        Task<ErrorCode> TransferAsync(TransferDto dto);
    }
}
