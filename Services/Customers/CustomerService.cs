using DatabaseLayer.DTOs;
using DatabaseLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Customers
{
    public class CustomerService : ICustomerService
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
                .ToListAsync();  return await _context.Customers
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

        public async Task<CustomerProfileDto> GetCustomerProfileAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return null!;

            return new CustomerProfileDto
            {
                CustomerId = customer.CustomerId,
                Gender = customer.Gender,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Streetaddress = customer.Streetaddress,
                City = customer.City,
                Zipcode = customer.Zipcode,
                Country = customer.Country,
                CountryCode = customer.CountryCode,
                Birthday = customer.Birthday,
                NationalId = customer.NationalId,
                Telephonecountrycode = customer.Telephonecountrycode,
                Telephonenumber = customer.Telephonenumber,
                Emailaddress = customer.Emailaddress,
                Accounts = customer.Dispositions
                    .Where(d => d.Account != null)
                    .Select(d => new AccountDTO
                    {
                        AccountId = d.Account.AccountId,
                        Frequency = d.Account.Frequency,
                        Created = d.Account.Created,
                        Balance = d.Account.Balance
                    }).ToList()
            };
        }

        public async Task<CustomerProfileDto?> GetCustomerByNationalIdAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
                return null;

            return new CustomerProfileDto
            {
                CustomerId = customer.CustomerId,
                Gender = customer.Gender,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Streetaddress = customer.Streetaddress,
                City = customer.City,
                Zipcode = customer.Zipcode,
                Country = customer.Country,
                CountryCode = customer.CountryCode,
                Birthday = customer.Birthday,
                NationalId = customer.NationalId,
                Telephonecountrycode = customer.Telephonecountrycode,
                Telephonenumber = customer.Telephonenumber,
                Emailaddress = customer.Emailaddress,
                Accounts = customer.Dispositions
                    .Where(d => d.Account != null)
                    .Select(d => new AccountDTO
                    {
                        AccountId = d.Account.AccountId,
                        Frequency = d.Account.Frequency,
                        Created = d.Account.Created,
                        Balance = d.Account.Balance
                    }).ToList()
            };
        }
    }
}
