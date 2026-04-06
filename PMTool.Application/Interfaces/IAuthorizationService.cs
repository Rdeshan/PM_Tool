using PMTool.Domain.Entities;
using PMTool.Domain.Enums;

namespace PMTool.Application.Interfaces;

public interface IAuthorizationService
{
    Task<bool> HasRoleAsync(Guid userId, RoleType roleType, Guid? projectId = null);
    Task<bool> HasPermissionAsync(Guid userId, PermissionType permissionType);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<Role>> GetUserProjectRolesAsync(Guid userId, Guid projectId);
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> AssignRoleToUserAsync(Guid userId, RoleType roleType, Guid? projectId = null);
    Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}
