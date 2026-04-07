using PMTool.Application.DTOs.User;

namespace PMTool.Application.Interfaces;

public interface IUserAdminService
{
    Task<UserDTO?> GetUserByIdAsync(Guid id);
    Task<UserDTO?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    Task<IEnumerable<UserDTO>> GetActiveUsersAsync();
    Task<IEnumerable<UserDTO>> GetInactiveUsersAsync();
    Task<bool> InviteUserAsync(InviteUserRequest request, Guid invitedByUserId);
    Task<bool> CompleteAccountSetupAsync(string token, string password, string firstName, string lastName);
    Task<bool> DeactivateUserAsync(Guid userId);
    Task<bool> ReactivateUserAsync(Guid userId);
    Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task<bool> AddUserToTeamAsync(Guid userId, Guid teamId);
    Task<bool> RemoveUserFromTeamAsync(Guid userId, Guid teamId);
    Task<IEnumerable<UserDTO>> GetUsersByTeamAsync(Guid teamId);
}
