using DatabaseLayer.DTOs;
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

        public List<CountriesStatisticsDTO> GetCountryStatistics()
        {
            return _context.Customers
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
                .ToList();
        }
    }
}
