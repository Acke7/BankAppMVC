using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer.DTOs;
using DatabaseLayer.Models;

namespace Services.Customers
{
    public interface ICustomerService
    {
      public  Task<List<CustomerListDto>> GetAllCustomersAsync();
      public  Task<CustomerProfileDto> GetCustomerProfileAsync(int customerId);
        Task<CustomerProfileDto?> GetCustomerByNationalIdAsync(int id);
    }
}
