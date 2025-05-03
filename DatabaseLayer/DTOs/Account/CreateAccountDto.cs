using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Account
{
    public class CreateAccountDto
    {
        public int CustomerId { get; set; }  // Needed to find disposition
        public string Frequency { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
