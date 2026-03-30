using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserRole>> GetByUserAndProjectAsync(Guid userId, Guid? projectId);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<Role>> GetUserProjectRolesAsync(Guid userId, Guid projectId);
    Task<bool> AssignRoleAsync(UserRole userRole);
    Task<bool> RemoveRoleAsync(Guid userRoleId);
    Task<bool> UpdateAsync(UserRole userRole);
    Task<bool> HasRoleAsync(Guid userId, int roleType, Guid? projectId = null);
}
