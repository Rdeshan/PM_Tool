using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMTool.Application.DTOs.Sprint;

namespace PMTool.Application.Interfaces;

public interface ISprintService
{
    Task<List<SprintDTO>> GetSprintsByProductAsync(Guid productId);
    Task<SprintDTO> CreateSprintAsync(CreateSprintRequest request);
    Task<bool> UpdateSprintAsync(SprintDTO sprint);
    Task<bool> DeleteSprintAsync(Guid id);
    Task<bool> MoveToSprintAsync(Guid itemId, Guid? sprintId, Guid userId);
}
