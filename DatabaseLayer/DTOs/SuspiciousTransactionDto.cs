using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs
{
    public class SuspiciousTransactionDto
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public string Country { get; set; }
        public DateTime DetectedAt { get; set; }

        public decimal Amount { get; set; }
        public int AccountId { get; set; }

        public string FullName { get; set; } = null!;
    }

}
