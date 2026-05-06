using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface ISprintRepository
{
    Task<Sprint?> GetByIdAsync(Guid id);
    Task<List<Sprint>> GetByProductIdAsync(Guid productId);
    Task<Sprint?> GetActiveByProductIdAsync(Guid productId);
    Task<Sprint> AddAsync(Sprint sprint);
    Task UpdateAsync(Sprint sprint);
    Task DeleteAsync(Guid id);
    Task AddScopeChangeAsync(SprintScopeChange change);
}
