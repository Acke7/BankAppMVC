using Microsoft.AspNetCore.Mvc;

namespace BankAppMVC.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            // return kundbild view
        }
    }
}
