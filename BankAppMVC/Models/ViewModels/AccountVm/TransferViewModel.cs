namespace BankAppMVC.Models.ViewModels.AccountVm
{
    public class TransferViewModel
    {
        public int AccountId { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Operation { get; set; } = "Transfer";
        public string? Symbol { get; set; }
    }
}
