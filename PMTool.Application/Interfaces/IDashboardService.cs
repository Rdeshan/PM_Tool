using System;
using System.Collections.Generic;
using System.Text;
using PMTool.Application.DTOs.Dashboard;

namespace PMTool.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(string userId);
    }
}
