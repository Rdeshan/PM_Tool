using PMTool.Application.DTOs.Backlog;

namespace PMTool.Application.Interfaces;

public interface IWorkTypeService
{
    Task<List<WorkTypeDTO>> GetCustomWorkTypesAsync();
    Task<WorkTypeDTO?> CreateWorkTypeAsync(CreateWorkTypeRequest request);
}
