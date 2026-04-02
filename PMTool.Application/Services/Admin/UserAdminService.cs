using PMTool.Application.DTOs.User;
using PMTool.Application.Services.User;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace PMTool.Application.Services.Admin;

public class UserAdminService : IUserAdminService
{
    private readonly IUserAdminRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public UserAdminService(
        IUserAdminRepository userRepository,
        IUserRoleRepository userRoleRepository,
        ITeamRepository teamRepository,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _teamRepository = teamRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDTO(user);
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : MapToDTO(user);
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<UserDTO>> GetActiveUsersAsync()
    {
        var users = await _userRepository.GetActiveAsync();
        return users.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<UserDTO>> GetInactiveUsersAsync()
    {
        var users = await _userRepository.GetInactiveAsync();
        return users.Select(MapToDTO).ToList();
    }

    public async Task<bool> InviteUserAsync(InviteUserRequest request, Guid invitedByUserId)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return false;

        var invitationToken = _tokenService.GenerateRandomToken();
        var user = new Domain.Entities.User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            EmailConfirmed = false,
            AccountSetupCompleted = false,
            InvitationToken = _tokenService.HashPassword(invitationToken),
            InvitationTokenExpiry = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userRepository.CreateAsync(user);
        if (!result)
            return false;

        // Assign roles
        foreach (var roleId in request.RoleIds)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            };
            await _userRoleRepository.AssignRoleAsync(userRole);
        }

        // Add to teams
        foreach (var teamId in request.TeamIds)
        {
            await _teamRepository.AddMemberAsync(teamId, user.Id);
        }

        // Send invitation email
        var setupLink = $"{_configuration["AppUrl"]}/Auth/SetupAccount?token={System.Web.HttpUtility.UrlEncode(invitationToken)}&email={System.Web.HttpUtility.UrlEncode(request.Email)}";
        await _emailService.SendAccountInvitationAsync(request.Email, setupLink);

        return true;
    }

    public async Task<bool> CompleteAccountSetupAsync(string token, string password, string firstName, string lastName)
    {
        // Note: This would need email parameter and token verification logic
        // Implementation depends on your email verification approach
        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        return await _userRepository.DeactivateAsync(userId);
    }

    public async Task<bool> ReactivateUserAsync(Guid userId)
    {
        return await _userRepository.ReactivateAsync(userId);
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DisplayName = request.DisplayName;
        user.AvatarUrl = request.AvatarUrl;
        user.NotificationsEnabled = request.NotificationsEnabled;

        return await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };
        return await _userRoleRepository.AssignRoleAsync(userRole);
    }

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var userRole = await _userRoleRepository.GetByUserIdAsync(userId);
        var roleToRemove = userRole.FirstOrDefault(ur => ur.RoleId == roleId);
        if (roleToRemove == null)
            return false;

        return await _userRoleRepository.RemoveRoleAsync(roleToRemove.Id);
    }

    public async Task<bool> AddUserToTeamAsync(Guid userId, Guid teamId)
    {
        return await _teamRepository.AddMemberAsync(teamId, userId);
    }

    public async Task<bool> RemoveUserFromTeamAsync(Guid userId, Guid teamId)
    {
        return await _teamRepository.RemoveMemberAsync(teamId, userId);
    }

    public async Task<IEnumerable<UserDTO>> GetUsersByTeamAsync(Guid teamId)
    {
        var users = await _userRepository.GetUsersByTeamAsync(teamId);
        return users.Select(MapToDTO).ToList();
    }

    private UserDTO MapToDTO(Domain.Entities.User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            NotificationsEnabled = user.NotificationsEnabled,
            EmailConfirmed = user.EmailConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            DeactivatedAt = user.DeactivatedAt,
            Roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new(),
            Teams = user.TeamMembers?.Select(tm => tm.Team?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new()
        };
    }
}
