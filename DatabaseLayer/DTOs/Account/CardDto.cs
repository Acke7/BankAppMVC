using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Account
{
    public class CardDto
    {
        public int CardId { get; set; }
        public string Type { get; set; } = null!;
        public DateOnly Issued { get; set; }

        public string Cctype { get; set; } = null!;

        public string Ccnumber { get; set; } = null!;

        public string Cvv2 { get; set; } = null!;

        public int ExpM { get; set; }

        public int ExpY { get; set; }
    }
}
