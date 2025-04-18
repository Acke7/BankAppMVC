namespace BankAppMVC.Models.ViewModels
{
    public class TransferViewModel
    {
        public int FromAccountNumber { get; set; }
        public int ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Operation { get; set; } = "Transfer";
        public string? Symbol { get; set; }
    }
}
