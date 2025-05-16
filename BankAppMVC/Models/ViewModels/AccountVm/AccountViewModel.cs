namespace BankAppMVC.Models.ViewModels.AccountVm
{
    public class AccountViewModel
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public int AccountNumber { get; set; }
        public string? Frequency { get; set; } = null!;
        public DateOnly Created { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public string? FormattedCreated => Created.ToString("yyyy-MM-dd");
        public string? FormattedBalance => Balance.ToString("C");
    }
}
