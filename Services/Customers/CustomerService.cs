using AutoMapper;
using DatabaseLayer.DTOs.Account;
using DatabaseLayer.DTOs.Customer;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly BankAppDataContext _context;
        private readonly IMapper _mapper;

        public CustomerService(BankAppDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<string>> GetAllCountriesAsync()
        {
            return await _context.Customers
                .Select(c => c.Country)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Customer>> GetCustomersByCountryAsync(string country)
        {
            return await _context.Customers
                .Where(c => c.Country == country)
                .ToListAsync();
        }

        public async Task<List<CustomerListDto>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
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
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Cards)
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
                    .Where(d => d.Account?.IsActive == true)
                    .Select(d => new AccountDTO
                    {
                        AccountId = d.Account.AccountId,
                        Frequency = d.Account.Frequency,
                        Created = d.Account.Created,
                        Balance = d.Account.Balance
                    }).ToList(),

                LinkedCards = customer.Dispositions
                    .SelectMany(d => d.Cards)
                    .Select(c => new CardDto
                    {
                        CardId = c.CardId,
                        Type = c.Type,
                        Issued = c.Issued,
                        Cctype = c.Cctype,
                        Ccnumber = c.Ccnumber,
                        Cvv2 = c.Cvv2,
                        ExpM = c.ExpM,
                        ExpY = c.ExpY
                    }).ToList()
            };
        }

        public async Task<CustomerProfileDto?> GetCustomerByNationalIdAsync(int id)
        {
            return await GetCustomerProfileAsync(id);
        }

        public async Task<List<CustomerDto>> GetAllActiveAsync()
        {
            var customers = await _context.Customers
                .Where(c => c.IsActive)
                .ToListAsync();

            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer != null ? _mapper.Map<CustomerDto>(customer) : null;
        }

        public async Task<bool> UpdateAsync(CustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null || !customer.IsActive)
                return false;

            customer.Givenname = dto.Givenname;
            customer.Surname = dto.Surname;
            customer.Streetaddress = dto.Streetaddress;
            customer.City = dto.City;
            customer.Zipcode = dto.Zipcode;
            customer.Country = dto.Country;
            customer.CountryCode = dto.CountryCode;
            customer.Birthday = dto.Birthday;
            customer.NationalId = dto.NationalId;
            customer.Telephonecountrycode = dto.Telephonecountrycode;
            customer.Telephonenumber = dto.Telephonenumber;
            customer.Emailaddress = dto.Emailaddress;
            customer.Gender = dto.Gender;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateAsync(CustomerDto dto)
        {
            var customer = new Customer
            {
                Gender = dto.Gender,
                Givenname = dto.Givenname,
                Surname = dto.Surname,
                Streetaddress = dto.Streetaddress,
                City = dto.City,
                Zipcode = dto.Zipcode,
                Country = dto.Country,
                CountryCode = dto.CountryCode,
                Birthday = dto.Birthday,
                NationalId = dto.NationalId,
                Telephonecountrycode = dto.Telephonecountrycode,
                Telephonenumber = dto.Telephonenumber,
                Emailaddress = dto.Emailaddress,
                IsActive = true
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            customer.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
