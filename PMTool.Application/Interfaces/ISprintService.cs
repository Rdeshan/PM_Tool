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

    /// <summary>Activates a sprint (Status → 2).</summary>
    Task<SprintDTO?> StartSprintAsync(Guid sprintId, DateTime startDate, DateTime endDate);

    /// <summary>Completes a sprint (Status → 3).</summary>
    Task<bool> CompleteSprintAsync(Guid sprintId);

    /// <summary>Returns the single active sprint for a product, or null.</summary>
    Task<SprintDTO?> GetActiveSprintAsync(Guid productId);
}
