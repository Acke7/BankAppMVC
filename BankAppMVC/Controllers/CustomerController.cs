using BankAppMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BankAppMVC.Models;
using Services.Customers;


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
    }

}

