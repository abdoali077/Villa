using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Dtos;

namespace Villla.Application.Services.Interface
{
    public interface IDashboardService
    {
        Task<RadialBarChartDto> GetBookingsDataAsync();
        Task<RadialBarChartDto> GetUsersDataAsync();
        Task<RevenueRadialChartDto> GetRevenueDataAsync();
        Task<PieChartDto> GetCustomerPieChartAsync();
    }
}
