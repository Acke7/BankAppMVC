using DataAccessLayer.Models;
using DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICustomerService
    {
        List<CustomerDTO> GetCustomers();

    }
}
