using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.DTOs.Customer
{
    public class CustomerListDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? NationalId { get; set; }



     
    }
}
