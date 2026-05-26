using PMTool.Application.DTOs.Dashboard;

namespace PMTool.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(string userId);
        Task<PersonalDashboardDto> GetPersonalDashboardAsync(string userId);
    }
}
