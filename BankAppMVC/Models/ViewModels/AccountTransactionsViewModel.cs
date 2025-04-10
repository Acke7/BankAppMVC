namespace BankAppMVC.Models.ViewModels
{
    public class AccountTransactionsViewModel
    {
        public int AccountNumber { get; set; }
        public decimal AccountBalance { get; set; }
        public DateOnly Date { get; set; }

        public string Type { get; set; } = null!;

        public string Operation { get; set; } = null!;

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public string? Symbol { get; set; }

        public string? Bank { get; set; }

        public string? Account { get; set; }
    }
}
