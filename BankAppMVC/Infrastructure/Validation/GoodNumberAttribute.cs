using System.ComponentModel.DataAnnotations;

namespace BankAppMVC.Infrastructure.Validation
{
    public class GoodNumberAttribute : ValidationAttribute
    {
        public GoodNumberAttribute()
        {
            ErrorMessage =
                "Det var INTE en bra siffra. Skriv 25, 50, 75 eller 100";
        }

        protected override ValidationResult? IsValid
            (object? value, ValidationContext validationContext)
        {
            int year = int.Parse(value.ToString());
            var goodAges = new[]{
                25, 50, 75, 100
            };
            if (goodAges.Contains(year))
                return ValidationResult.Success;
            return
                new ValidationResult(ErrorMessage);
        }
    }

}
