using BankAppMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BankAppMVC.Models;
using Services.Customers;
using Humanizer;


namespace BankAppMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index(string sortOrder = "name_asc", int page = 1, string? name = null, string? city = null,string? search = null)
        {
            int pageSize = 50;

            var allCustomers = await _customerService.GetAllCustomersAsync();
            // 🔍 Filter by name
            if (!string.IsNullOrWhiteSpace(name))
            {
                allCustomers = allCustomers
                    .Where(c => c.FullName.ToLower().Contains(name.ToLower()))
                    .ToList();
            }

            // 🔍 Filter by city
            if (!string.IsNullOrWhiteSpace(city))
            {
                allCustomers = allCustomers
                    .Where(c => !string.IsNullOrEmpty(c.City) && c.City.ToLower().Contains(city.ToLower()))
                    .ToList();
            }

            //Search
            
                if (!string.IsNullOrWhiteSpace(search))
                {
                    allCustomers = allCustomers
                        .Where(c =>
                            (!string.IsNullOrEmpty(c.FullName) && c.FullName.ToLower().Contains(search.ToLower())) ||
                            (!string.IsNullOrEmpty(c.City) && c.City.ToLower().Contains(search.ToLower())) ||
                             (!string.IsNullOrEmpty(c.StreetAddress) && c.StreetAddress.ToLower().Contains(search.ToLower())) ||
                            (!string.IsNullOrEmpty(c.NationalId) && c.NationalId.ToLower().Contains(search.ToLower()))
                        )
                        .ToList();
                }
           


            // Sort
            allCustomers = sortOrder switch
            {
                "name_desc" => allCustomers.OrderByDescending(c => c.FullName).ToList(),
                "city_asc" => allCustomers.OrderBy(c => c.City).ToList(),
                "city_desc" => allCustomers.OrderByDescending(c => c.City).ToList(),
                _ => allCustomers.OrderBy(c => c.FullName).ToList(), // default
            };

            // Paging
            int totalCount = allCustomers.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedCustomers = allCustomers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new CustomerIndexViewModel
            {
                Customers = pagedCustomers,
                CurrentPage = page,
                TotalPages = totalPages,
                SortOrder = sortOrder
            };

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // Return the view without any model (clean search page)
                return View(model: null);
            }

            var customer = await _customerService.GetCustomerProfileAsync(id.Value);
            if (customer == null)
            {
                TempData["Error"] = $"Customer with ID {id.Value} not found.";
                return View(model: null); // show just the search box
            }

            var vm = new CustomerProfileViewModel
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
                Accounts = customer.Accounts.Select(a => new AccountViewModel
                {
                   
                    AccountNumber = a.AccountNumber,
                    Frequency = a.Frequency,
                    Created = a.Created,
                    Balance = a.Balance
                }).ToList()
            };

            return View(vm); // same Details.cshtml
        }



        [HttpPost]
        public async Task<IActionResult>  FindByCustomerId(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Please enter a valid customer ID.";
                return View("Details"); // or a search page
            }

            var dto = await _customerService.GetCustomerProfileAsync(id);
            if (dto == null)
            {
                TempData["Error"] = $"No customer found with ID {id}.";
                return View("Details");
            }

            var viewModel = new CustomerProfileViewModel
            {
                CustomerId = dto.CustomerId,
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

                Accounts = dto.Accounts.Select(a => new AccountViewModel
                {
                  
                    Frequency = a.Frequency,
                    Created = a.Created,
                    Balance = a.Balance
                }).ToList()
            };

            return RedirectToAction("Details", new { id });
        }

    }




}

