using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BankAppMVC.Models.ViewModels.AccountVm
{

    [BindProperties]
    public class DepositViewModel
    {
        public int AccountId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "The field {0} must be a number.")]
        [RegularExpression(@"^\d{1,7}(\.\d{1,2})?$", ErrorMessage = "The field {0} must not exceed 7 digits.")]
        public decimal Amount { get; set; }
        public string Operation { get; set; } = "Credit in Cash";

        [MaxLength(20)]
        public string? Symbol { get; set; }
    }
}
