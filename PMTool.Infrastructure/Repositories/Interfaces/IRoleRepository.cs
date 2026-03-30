using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<Role?> GetByTypeAsync(int roleType);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetActiveAsync();
    Task<bool> CreateAsync(Role role);
    Task<bool> UpdateAsync(Role role);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Permission>> GetPermissionsAsync(Guid roleId);
}
