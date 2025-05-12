using DatabaseLayer.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer.Models;


namespace Services.Account
{
    public interface IAccountService
    {
        public  Task<List<AccountTransaktionDto>> GetTransactionsByAccountNumber(int accountId);
        public Task<decimal> GetBalanceByAccountId(int accountId);

        Task<AccountDTO> GetAccountByAccountNumber(int accountId);
        Task CreateAccount(CreateAccountDto dto);
        Task UpdateAccount(int accountId, UpdateAccountDto dto);
        Task DeleteAccount(int accountId);
        Task<List<DatabaseLayer.Models.Account>> GetAccountsByCustomerIdAsync(int customerId);

    }
}
