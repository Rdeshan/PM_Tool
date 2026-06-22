using PMTool.Application.DTOs.Progress;

namespace PMTool.Application.Interfaces;

public interface IProgressService
{
    Task<DashboardProgressDto> GetDashboardProgressAsync();
    Task<ProductAnalysisDto> GetProductAnalysisAsync(Guid productId);
}
