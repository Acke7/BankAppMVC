using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer.DTOs.Customer;

namespace Services.Statistics
{
    public interface IStatisticsService
    {
        Task< List<CountriesStatisticsDTO> >GetCountryStatistics();
    }
}
