using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Transaktion
{
    public class TransferDto
    {
        public int FromAccountNumber { get; set; }
        public int ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Operation { get; set; } = "Transfer";
        public string? Symbol { get; set; }
        public string? Bank { get; set; }
        public string? Account { get; set; }
    }
}
