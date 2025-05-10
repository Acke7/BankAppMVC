using DatabaseLayer.DTOs.Customer;
using DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly BankAppDataContext _context;

        public StatisticsService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<List<TopCustomerDTO>> GetTopCustomersByCountryAsync(string country)
        {
            var result = await _context.Customers
                .Where(c => c.Country == country)
                .SelectMany(c => c.Dispositions
                    .Select(d => new
                    {
                        c.CustomerId,
                        c.Givenname,
                        c.Surname,
                        AccountBalance = d.Account.Balance
                    }))
                .GroupBy(x => new { x.CustomerId, x.Givenname, x.Surname })
                .Select(g => new TopCustomerDTO
                {
                    CustomerId = g.Key.CustomerId,
                    Givenname = g.Key.Givenname,
                    Surname = g.Key.Surname,
                    TotalBalance = g.Sum(x => x.AccountBalance)
                })
                .OrderByDescending(dto => dto.TotalBalance)
                .Take(10)
                .ToListAsync();

            return result;
        }
        public async Task< List<CountriesStatisticsDTO> >GetCountryStatistics()
        {
            return await _context.Customers
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                .GroupBy(c => c.Country)
                .Select(g => new CountriesStatisticsDTO
                {
                    Country = g.Key,
                    CustomerCount = g.Count(),
                    AccountCount = g.SelectMany(c => c.Dispositions)
                                    .Select(d => d.AccountId)
                                    .Distinct()
                                    .Count(),
                    TotalBalance = g.SelectMany(c => c.Dispositions)
                                    .Select(d => d.Account)
                                    .Distinct()
                                    .Sum(a => a.Balance)
                })
                  .ToListAsync();
        }
    }
}
