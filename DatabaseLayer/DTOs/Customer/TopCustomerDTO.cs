using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Customer
{
    public class TopCustomerDTO
    {
        public int CustomerId { get; set; }
        public string Givenname { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
    }
}
