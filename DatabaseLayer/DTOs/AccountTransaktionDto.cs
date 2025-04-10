using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs
{
    public class AccountTransaktionDto
    {
         public int AccountNumber { get; set; }
        public DateOnly Date { get; set; }

        public string Type { get; set; } = null!;

        public string Operation { get; set; } = null!;

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public string? Symbol { get; set; }

        public string? Bank { get; set; }

        public string? Account { get; set; }
    }
}
