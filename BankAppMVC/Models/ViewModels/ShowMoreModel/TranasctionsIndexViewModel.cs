using DatabaseLayer.DTOs.Account;

namespace BankAppMVC.Models.ViewModels.ShowMoreModel
{
    public class TranasctionsIndexViewModel
    {
        public List<AccountTransaktionDto> Transactions { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
       
    }
}
