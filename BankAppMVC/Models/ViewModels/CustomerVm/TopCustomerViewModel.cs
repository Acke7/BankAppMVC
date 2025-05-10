namespace BankAppMVC.Models.ViewModels.CustomerVm
{
    public class TopCustomerViewModel
    {
        public string FullName => $"{Givenname} {Surname}";
        public decimal TotalBalance { get; set; }
        public int CustomerId { get; set; }
        public string Givenname { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
    }
}
