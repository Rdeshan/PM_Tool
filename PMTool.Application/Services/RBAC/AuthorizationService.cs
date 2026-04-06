using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.RBAC;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public AuthorizationService(
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<bool> HasRoleAsync(Guid userId, RoleType roleType, Guid? projectId = null)
    {
        return await _userRoleRepository.HasRoleAsync(userId, (int)roleType, projectId);
    }

    public async Task<bool> HasPermissionAsync(Guid userId, PermissionType permissionType)
    {
        return await _permissionRepository.HasPermissionAsync(userId, (int)permissionType);
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _userRoleRepository.GetUserRolesAsync(userId);
    }

    public async Task<IEnumerable<Role>> GetUserProjectRolesAsync(Guid userId, Guid projectId)
    {
        return await _userRoleRepository.GetUserProjectRolesAsync(userId, projectId);
    }

    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        var userRoles = await GetUserRolesAsync(userId);
        var permissions = new HashSet<Permission>();

        foreach (var role in userRoles)
        {
            var rolePermissions = await _roleRepository.GetPermissionsAsync(role.Id);
            foreach (var permission in rolePermissions)
            {
                permissions.Add(permission);
            }
        }

        return permissions;
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, RoleType roleType, Guid? projectId = null)
    {
        var role = await _roleRepository.GetByTypeAsync((int)roleType);
        if (role == null)
            return false;

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            ProjectId = projectId,
            IsActive = true
        };

        return await _userRoleRepository.AssignRoleAsync(userRole);
    }

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
        var userRole = userRoles.FirstOrDefault(ur => ur.RoleId == roleId);

        if (userRole == null)
            return false;

        return await _userRoleRepository.RemoveRoleAsync(userRole.Id);
    }
}
