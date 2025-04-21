using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BankAppMVC.Models.ViewModels.AccountVm
{

    [BindProperties]
    public class DepositViewModel
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Operation { get; set; } = "Credit in Cash";

        [MaxLength(20)]
        public string? Symbol { get; set; }
    }
}
