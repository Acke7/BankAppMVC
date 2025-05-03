using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Account
{
    public class UpdateAccountDto
    {
        public int AccountId { get; set; }
        public string Frequency { get; set; } = null!;
        public decimal Balance { get; set; }
        public int AccountNumber { get; set; }
    }
}
