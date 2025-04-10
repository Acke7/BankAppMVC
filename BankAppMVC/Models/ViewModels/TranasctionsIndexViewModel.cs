using DatabaseLayer.DTOs;

namespace BankAppMVC.Models.ViewModels
{
    public class TranasctionsIndexViewModel
    {
        public List<AccountTransaktionDto> Transactions { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
       
    }
}
