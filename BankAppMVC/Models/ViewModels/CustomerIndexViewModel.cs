using DatabaseLayer.DTOs.Customer;

namespace BankAppMVC.Models.ViewModels
{
    public class CustomerIndexViewModel
    {
        public List<CustomerListDto> Customers { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortOrder { get; set; } = "name_asc";
    }
}
