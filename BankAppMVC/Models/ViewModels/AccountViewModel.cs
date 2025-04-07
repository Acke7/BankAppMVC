namespace BankAppMVC.Models.ViewModels
{
    public class AccountViewModel
    {
        public int AccountId { get; set; }
        public string Frequency { get; set; } = null!;
        public DateOnly Created { get; set; }
        public decimal Balance { get; set; }

        public string FormattedCreated => Created.ToString("yyyy-MM-dd");
        public string FormattedBalance => Balance.ToString("C");
    }
}
