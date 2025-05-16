using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer.DTOs.Customer;
using DatabaseLayer.Models;

namespace Services.Customers
{
    public interface ICustomerService
    {
        public Task<List<CustomerListDto>> GetAllCustomersAsync();
        public Task<CustomerProfileDto> GetCustomerProfileAsync(int customerId);
        Task<CustomerProfileDto?> GetCustomerByNationalIdAsync(int id);
        Task<bool> CreateAsync(CustomerDto dto);
        Task<List<CustomerDto>> GetAllActiveAsync();
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(CustomerDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<List<string>> GetAllCountriesAsync();
        Task<List<Customer>> GetCustomersByCountryAsync(string country);
    }
}
