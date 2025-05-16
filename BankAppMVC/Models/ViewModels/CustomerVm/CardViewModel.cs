namespace BankAppMVC.Models.ViewModels.CustomerVm
{
    public class CardViewModel
    {
        public int CardId { get; set; }
        public string Type { get; set; } = null!;
        public DateOnly Issued { get; set; }

        public string Cctype { get; set; } = null!;

        public string Ccnumber { get; set; } = null!;

        public string Cvv2 { get; set; } = null!;

        public int ExpM { get; set; }

        public int ExpY { get; set; }

        public string MaskedNumber => $"**** **** **** {Ccnumber[^4..]}";
        public string Expiry => $"{ExpM:D2}/{ExpY}";
    }
}
