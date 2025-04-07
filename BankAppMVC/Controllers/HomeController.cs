using System.Diagnostics;
using BankAppMVC.Models;
using BankAppMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Statistics;

namespace BankAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public HomeController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }


        public async Task< IActionResult >Index()
        {
            var swedenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var swedenTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, swedenTimeZone);
            ViewBag.SwedenTime = swedenTime; ;
            var dtoList = await _statisticsService.GetCountryStatistics();

            var viewModelList = dtoList.Select(dto => new CountryStatsViewModel
            {
                Country = dto.Country,
                Customers = dto.CustomerCount,
                Accounts = dto.AccountCount,
                TotalSaldo = dto.TotalBalance
            }).ToList();

            return View(viewModelList);
        }
    }
}