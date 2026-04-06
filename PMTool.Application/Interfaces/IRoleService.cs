using PMTool.Domain.Entities;
using PMTool.Domain.Enums;

namespace PMTool.Application.Interfaces;

public interface IRoleService
{
    Task<Role?> GetRoleByIdAsync(Guid id);
    Task<Role?> GetRoleByTypeAsync(RoleType roleType);
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<IEnumerable<Role>> GetActiveRolesAsync();
    Task<bool> CreateRoleAsync(Role role);
    Task<bool> UpdateRoleAsync(Role role);
    Task<bool> DeleteRoleAsync(Guid roleId);
    Task<IEnumerable<Permission>> GetRolePermissionsAsync(Guid roleId);
    Task InitializeDefaultRolesAsync();
}
