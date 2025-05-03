namespace BankAppMVC.Models.ViewModels.AccountVm
{
    public class CreateAccountViewModel
    {
        public int CustomerId { get; set; }
        public string Frequency { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
