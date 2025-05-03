namespace BankAppMVC.Models.ViewModels.AccountVm
{
    public class UpdateAccountViewModel
    {
       
        public int AccountId { get; set; }        // Which account you are updating
        public int CustomerId { get; set; }        // To know where to return after update
        public string Frequency { get; set; } = null!;
        public decimal Balance { get; set; }
        public int AccountNumber { get; set; }
    }
}
