using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Transaktion
{
    public class TransactionDto
    {
        
            public int AccountNumber { get; set; }
            public decimal Amount { get; set; }
            public string Operation { get; set; }
            public string Type { get; set; } // "Credit"/"Debit"
            public DateTime Date { get; set; }
       
    }
}
