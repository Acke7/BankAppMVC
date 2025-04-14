using DatabaseLayer.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Account
{
    public interface IAccountService
    {
        public  Task<List<AccountTransaktionDto>> GetTransactionsByAccountNumber(int accountId);
        public Task<decimal> GetBalanceByAccountId(int accountId);
        
      
    }
}
