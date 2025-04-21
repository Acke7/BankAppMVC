namespace BankAppMVC.Models.ViewModels.AccountVm
{
    public class WithdrawViewModel
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Operation { get; set; } = "Withdraw";
        public string? Symbol { get; set; }
    }
}
