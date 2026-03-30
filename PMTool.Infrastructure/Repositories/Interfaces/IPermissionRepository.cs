using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id);
    Task<Permission?> GetByNameAsync(string name);
    Task<Permission?> GetByTypeAsync(int permissionType);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetActiveAsync();
    Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId);
    Task<bool> CreateAsync(Permission permission);
    Task<bool> UpdateAsync(Permission permission);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> HasPermissionAsync(Guid userId, int permissionType);
}
