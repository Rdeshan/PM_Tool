using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IUserAdminRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetActiveAsync();
    Task<IEnumerable<User>> GetInactiveAsync();
    Task<bool> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeactivateAsync(Guid userId);
    Task<bool> ReactivateAsync(Guid userId);
    Task<IEnumerable<User>> GetUsersByTeamAsync(Guid teamId);
    Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId);
}
