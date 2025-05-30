    using System.Diagnostics;
    using BankAppMVC.Models;
    using BankAppMVC.Models.ViewModels;
    using BankAppMVC.Models.ViewModels.CustomerVm;
    using Microsoft.AspNetCore.Mvc;
    using Services.Statistics;

    namespace BankAppMVC.Controllers
    {
        public class HomeController : Controller
        {
            private readonly IStatisticsService _statisticsService;
            private readonly ILogger<HomeController> _logger;
            public HomeController(IStatisticsService statisticsService, ILogger<HomeController> logger)
            {
                _statisticsService = statisticsService;
                _logger = logger;
            }
            [IgnoreAntiforgeryToken]
            [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" })]
            [HttpGet("/TopCustomers/{country}")]
            public async Task<IActionResult> TopCustomers(string country)
            {
                _logger.LogInformation($"TopCustomers action HIT for {country} at {DateTime.Now}");
                var dtoList = await _statisticsService.GetTopCustomersByCountryAsync(country);
                var viewModels = dtoList.Select(x => new TopCustomerViewModel
                {
                    CustomerId = x.CustomerId,
                    Givenname = x.Givenname,
                    Surname = x.Surname,
                    TotalBalance = x.TotalBalance
                }).ToList();

                ViewBag.Country = country;
                return View(viewModels);
            }


            public async Task< IActionResult >Index()
            {
                var swedenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                var swedenTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, swedenTimeZone);
                ViewBag.SwedenTime = swedenTime; ;
                var dtoList = await _statisticsService.GetCountryStatistics();

                var viewModelList = dtoList.Select(dto => new CountryStatsticsViewModel
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