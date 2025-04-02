namespace BankAppMVC.Models.ViewModels
{
    public class CountryStatsViewModel
    {
        public string Country { get; set; } = null!;
        public int Customers { get; set; }
        public int Accounts { get; set; }
        public decimal TotalSaldo { get; set; }
    }
}
