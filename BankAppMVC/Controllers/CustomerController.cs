using BankAppMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BankAppMVC.Models;
using Services.Customers;
using Humanizer;
using BankAppMVC.Models.ViewModels.Paging;
using BankAppMVC.Models.ViewModels.CustomerVm;
using BankAppMVC.Models.ViewModels.AccountVm;
using AutoMapper;
using DatabaseLayer.DTOs.Customer;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BankAppMVC.Controllers
{
    [BindProperties]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string sortOrder = "name_asc", int page = 1, string? name = null, string? city = null,string? search = null)
        {
            int pageSize = 50;

            var allCustomers = await _customerService.GetAllCustomersAsync();
            // Filter by name
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
                     
                        c.CustomerId.ToString().Contains(search) 
                     
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
                    AccountId = a.AccountId,
                    Frequency = a.Frequency,
                    Created = a.Created,
                    Balance = a.Balance
                }).ToList(),

                LinkedCards = customer.LinkedCards.Select(c => new CardViewModel
                {
                    CardId = c.CardId,
                    Type = c.Type,
                    Issued = c.Issued,
                    Cctype = c.Cctype,
                    Ccnumber = c.Ccnumber,
                    ExpM = c.ExpM,
                    ExpY = c.ExpY
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

        //public async Task<IActionResult> Index()
        //{
        //    var dtos = await _customerService.GetAllActiveAsync();
        //    var viewModels = _mapper.Map<List<CustomerViewModelCrud>>(dtos);
        //    return View(viewModels);
        //}

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _customerService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = _mapper.Map<CustomerViewModelCrud>(dto);
            ViewBag.GenderList = new SelectList(new[] { "Male", "Female" });
            ViewBag.CountryList = new SelectList(new[] { "Sweden", "Denmark", "Norway", "Finland" });

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomerViewModelCrud vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<CustomerDto>(vm);
            var result = await _customerService.UpdateAsync(dto);
            if (!result) return NotFound();

            TempData["ToastMessage"] = "Customer successfully Edited !";
            TempData["ToastType"] = "warning"; // use 'success', 'warning', or 'danger'
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            ViewBag.GenderList = new SelectList(new[] { "Male", "Female" });
            ViewBag.CountryList = new SelectList(new[] { "Sweden", "Denmark", "Norway", "Finland" });

            return View(new CustomerViewModelCrud());
        }

        [HttpPost]
      
        public async Task<IActionResult> Create(CustomerViewModelCrud vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.GenderList = new SelectList(new[] { "Male", "Female" });
                ViewBag.CountryList = new SelectList(new[] { "Sweden", "Denmark", "Norway", "Finland" });

                return View(vm);
            }

            var dto = _mapper.Map<CustomerDto>(vm);
            var result = await _customerService.CreateAsync(dto);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Failed to create customer.");
                return View(vm);
            }
            TempData["ToastMessage"] = "Customer successfully Added!";
            TempData["ToastType"] = "success"; // use 'success', 'warning', or 'danger'
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.SoftDeleteAsync(id);
           
            TempData["ToastMessage"] = "Customer successfully deleted!";
            TempData["ToastType"] = "danger"; // use 'success', 'warning', or 'danger'
            return RedirectToAction("Index");
        }
    }
}

