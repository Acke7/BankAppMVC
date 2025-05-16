using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.Models
{
    public class SuspiciousTransaction
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string Country { get; set; } = null!;
        public DateTime DetectedAt { get; set; }
    }
}
