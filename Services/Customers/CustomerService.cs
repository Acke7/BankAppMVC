using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private readonly BankAppDataContext _dbContext;

        public CustomerService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CustomerDTO> GetCustomers()
        {
            return _dbContext.Customers.
                Select(c => new CustomerDTO
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname
                }).
                ToList();
        }
    }
}
