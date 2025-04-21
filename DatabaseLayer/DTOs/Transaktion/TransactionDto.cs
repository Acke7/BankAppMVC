using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Transaktion
{
    public class TransactionDto
    {
        public int TransactionId { get; set; } 
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Operation { get; set; } 
        public string? Symbol { get; set; }
        public string? Bank { get; set; }
        public string? Account { get; set; }
    }
}
