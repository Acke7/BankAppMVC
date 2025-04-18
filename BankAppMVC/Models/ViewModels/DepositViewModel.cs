namespace BankAppMVC.Models.ViewModels
{
    public class DepositViewModel
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Operation { get; set; } = "Credit in Cash";

        public string? Symbol { get; set; }
    }
}
