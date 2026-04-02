using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Services.Interfaces;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Services;

public class DataSeedingService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public DataSeedingService(
        AppDbContext context,
        ITokenService tokenService,
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _context = context;
        _tokenService = tokenService;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task SeedTestUsersAsync()
    {
        // Test users data: (Email, FirstName, LastName, Password, RoleType)
        var testUsers = new List<(string Email, string FirstName, string LastName, string Password, RoleType RoleType)>
        {
            ("admin@pmtool.com", "Admin", "User", "Admin@123", RoleType.Administrator),
            ("pm@pmtool.com", "Project", "Manager", "PM@123", RoleType.ProjectManager),
            ("dev@pmtool.com", "Dev", "Eloper", "Dev@123", RoleType.Developer),
            ("qa@pmtool.com", "QA", "Engineer", "QA@123", RoleType.QAEngineer),
            ("ba@pmtool.com", "Business", "Analyst", "BA@123", RoleType.BusinessAnalyst),
            ("viewer@pmtool.com", "View", "Only", "Viewer@123", RoleType.Viewer),
            ("guest@pmtool.com", "Guest", "User", "Guest@123", RoleType.Guest)
        };

        foreach (var (email, firstName, lastName, password, roleType) in testUsers)
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                continue;

            // Create new user
            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = _tokenService.HashPassword(password),
                IsActive = true,
                EmailConfirmed = true,
                TwoFactorEnabled = false,
                FailedLoginAttempts = 0
            };

            var userCreated = await _userRepository.CreateAsync(user);
            if (!userCreated)
                continue;

            // Assign role to user
            var role = await _roleRepository.GetByTypeAsync((int)roleType);
            if (role != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    AssignedAt = DateTime.UtcNow
                };

                await _userRoleRepository.AssignRoleAsync(userRole);
            }
        }
    }
}
