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

    public async Task SeedDashboardDataAsync()
    {
        // Idempotent — skip if seed project already exists
        if (await _context.Projects.AnyAsync(p => p.ProjectCode == "SEED-DEV"))
            return;

        var devUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "dev@pmtool.com");
        var pmUser  = await _context.Users.FirstOrDefaultAsync(u => u.Email == "pm@pmtool.com");
        if (devUser == null || pmUser == null)
            return;

        var now = DateTime.UtcNow;

        // Project
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "PMTool Platform",
            Description = "Main product development project used for dashboard demo.",
            ClientName = "Internal",
            ProjectCode = "SEED-DEV",
            Status = 1, // Active
            ColourCode = "#0D6EFD",
            IsArchived = false,
            StartDate = now.AddMonths(-3),
            ExpectedEndDate = now.AddMonths(3),
            CreatedByUserId = pmUser.Id,
            CreatedAt = now.AddMonths(-3),
            UpdatedAt = now
        };
        _context.Projects.Add(project);

        // Product v1
        var product1 = new Product
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            VersionName = "v1.0 — Core",
            Description = "Initial release with core features.",
            PlannedReleaseDate = now.AddMonths(1),
            Status = 1,
            ReleaseType = 1,
            CreatedAt = now.AddMonths(-3),
            UpdatedAt = now
        };

        // Product v2
        var product2 = new Product
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            VersionName = "v2.0 — Dashboard",
            Description = "Personal dashboard and reporting features.",
            PlannedReleaseDate = now.AddMonths(3),
            Status = 1,
            ReleaseType = 2,
            CreatedAt = now.AddMonths(-1),
            UpdatedAt = now
        };
        _context.Products.AddRange(product1, product2);

        // Active Sprint linked to product2
        var sprint = new Sprint
        {
            Id = Guid.NewGuid(),
            ProductId = product2.Id,
            Name = "Sprint 3 — Dashboard MVP",
            Goal = "Ship the personal dashboard with stats and ticket table.",
            StartDate = now.AddDays(-7),
            EndDate = now.AddDays(7),
            Status = 2, // Active
            CreatedAt = now.AddDays(-7),
            UpdatedAt = now
        };
        _context.Sprints.Add(sprint);

        // Helper to create a backlog item
        ProductBacklog Ticket(Guid productId, string title, int status, int priority,
            DateTime? dueDate, DateTime? updatedAt = null, Guid? sprintId = null) => new()
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            OwnerId = devUser.Id,
            Title = title,
            Description = $"Seed data: {title}",
            Type = 1,
            Priority = priority,
            Status = status,
            DueDate = dueDate,
            StartDate = now.AddDays(-14),
            StoryPoints = priority switch { 1 => 8, 2 => 5, 3 => 3, _ => 1 },
            SprintId = sprintId,
            CreatedAt = now.AddDays(-20),
            UpdatedAt = updatedAt ?? now
        };

        var tickets = new List<ProductBacklog>
        {
            // To Do
            Ticket(product1.Id, "Set up CI/CD pipeline",           1, 1, now.AddDays(10)),
            Ticket(product1.Id, "Write unit tests for auth service", 1, 2, now.AddDays(5)),
            Ticket(product2.Id, "Design notifications panel",        1, 3, now.AddDays(8),  sprintId: sprint.Id),

            // In Progress (status 2)
            Ticket(product2.Id, "Build personal dashboard UI",       2, 1, now.AddDays(3),  sprintId: sprint.Id),
            Ticket(product2.Id, "Wire up ticket table filters",       2, 2, now.AddDays(2),  sprintId: sprint.Id),
            Ticket(product1.Id, "Implement JWT refresh token flow",   2, 2, now.AddDays(6)),

            // In Review (status 3)
            Ticket(product1.Id, "Code review: backlog status dropdown", 3, 3, now.AddDays(1)),

            // Done this week (status 4, updatedAt within last 3 days)
            Ticket(product2.Id, "Create DashboardDto DTOs",            4, 3, null, now.AddDays(-1), sprint.Id),
            Ticket(product2.Id, "Add GetPersonalDashboardAsync service", 4, 2, null, now.AddDays(-2), sprint.Id),

            // Done older (status 4, updatedAt > 7 days ago)
            Ticket(product1.Id, "Initial project scaffolding",          4, 3, null, now.AddDays(-15)),
            Ticket(product1.Id, "Database migrations setup",            4, 2, null, now.AddDays(-12)),

            // Overdue: status In Progress but DueDate is past
            Ticket(product1.Id, "Fix login redirect bug",               2, 1, now.AddDays(-3)),
            Ticket(product2.Id, "Sprint retrospective report",          1, 3, now.AddDays(-5), sprintId: sprint.Id),
        };
        _context.ProductBacklogs.AddRange(tickets);

        // Notifications for dev user
        var notifications = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), UserId = devUser.Id, Message = "You have been assigned to 'Build personal dashboard UI'", CreatedAt = now.AddHours(-1) },
            new() { Id = Guid.NewGuid(), UserId = devUser.Id, Message = "Sprint 3 — Dashboard MVP has started. You have 5 items assigned.", CreatedAt = now.AddDays(-7) },
            new() { Id = Guid.NewGuid(), UserId = devUser.Id, Message = "'Fix login redirect bug' is overdue — please update the status.", CreatedAt = now.AddDays(-1) },
            new() { Id = Guid.NewGuid(), UserId = devUser.Id, Message = "Code review requested on 'backlog status dropdown'.", CreatedAt = now.AddHours(-3) },
        };
        _context.Notifications.AddRange(notifications);

        await _context.SaveChangesAsync();
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
