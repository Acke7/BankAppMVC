using DatabaseLayer.DTOs;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Customers
{
    public class CustomerService: ICustomerService
    {
        private readonly BankAppDataContext _context;

        public CustomerService(BankAppDataContext context)
        {
            _context = context;
        }
        public async Task<List<CustomerListDto>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Select(c => new CustomerListDto
                {
                    CustomerId = c.CustomerId,
                    FullName = c.Givenname + " " + c.Surname,
                    City = c.City,
                    StreetAddress = c.Streetaddress, 
                    NationalId = c.NationalId
                })
                .ToListAsync();
        }

    }
}
