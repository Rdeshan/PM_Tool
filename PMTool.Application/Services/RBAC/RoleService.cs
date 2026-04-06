using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.RBAC;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<Role?> GetRoleByIdAsync(Guid id)
    {
        return await _roleRepository.GetByIdAsync(id);
    }

    public async Task<Role?> GetRoleByTypeAsync(RoleType roleType)
    {
        return await _roleRepository.GetByTypeAsync((int)roleType);
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await _roleRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync()
    {
        return await _roleRepository.GetActiveAsync();
    }

    public async Task<bool> CreateRoleAsync(Role role)
    {
        return await _roleRepository.CreateAsync(role);
    }

    public async Task<bool> UpdateRoleAsync(Role role)
    {
        return await _roleRepository.UpdateAsync(role);
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId)
    {
        return await _roleRepository.DeleteAsync(roleId);
    }

    public async Task<IEnumerable<Permission>> GetRolePermissionsAsync(Guid roleId)
    {
        return await _roleRepository.GetPermissionsAsync(roleId);
    }

    public async Task InitializeDefaultRolesAsync()
    {
        // This would be called during app startup to create default roles
        var roles = new Dictionary<RoleType, (string Name, string Description, List<PermissionType> Permissions)>
        {
            {
                RoleType.Administrator,
                (
                    "Administrator",
                    "Full system access; manages users, projects, and organisation-wide settings",
                    new List<PermissionType>
                    {
                        PermissionType.ManageUsers, PermissionType.ManageRoles, PermissionType.ViewUsers,
                        PermissionType.ManageOrganization, PermissionType.ManageOrganizationSettings,
                        PermissionType.ViewSystemLogs, PermissionType.ManageSystemSettings
                    }
                )
            },
            {
                RoleType.ProjectManager,
                (
                    "Project Manager",
                    "Creates and manages projects, products, sub-projects, sprints, milestones, and team assignments",
                    new List<PermissionType>
                    {
                        PermissionType.CreateProject, PermissionType.EditProject, PermissionType.DeleteProject,
                        PermissionType.ViewProject, PermissionType.ManageProjectMembers,
                        PermissionType.CreateProduct, PermissionType.EditProduct, PermissionType.DeleteProduct,
                        PermissionType.CreateSubProject, PermissionType.EditSubProject, PermissionType.DeleteSubProject,
                        PermissionType.CreateSprint, PermissionType.EditSprint, PermissionType.DeleteSprint,
                        PermissionType.CreateMilestone, PermissionType.EditMilestone, PermissionType.DeleteMilestone,
                        PermissionType.ViewReports, PermissionType.ViewDashboards
                    }
                )
            },
            {
                RoleType.Developer,
                (
                    "Developer",
                    "Creates and updates tickets, logs time, and posts comments within assigned sub-projects",
                    new List<PermissionType>
                    {
                        PermissionType.ViewProject, PermissionType.ViewSubProject, PermissionType.ViewSprint,
                        PermissionType.CreateTicket, PermissionType.EditTicket, PermissionType.ViewTicket,
                        PermissionType.UpdateTicketStatus, PermissionType.LogTime, PermissionType.PostComment,
                        PermissionType.ViewReports, PermissionType.ViewDashboards
                    }
                )
            },
            {
                RoleType.QAEngineer,
                (
                    "QA Engineer",
                    "Creates and manages bug tickets, links test cases, and updates ticket status within assigned sub-projects",
                    new List<PermissionType>
                    {
                        PermissionType.ViewProject, PermissionType.ViewSubProject, PermissionType.ViewSprint,
                        PermissionType.CreateBugTicket, PermissionType.EditBugTicket, PermissionType.DeleteBugTicket,
                        PermissionType.LinkTestCases, PermissionType.UpdateTicketStatus,
                        PermissionType.ViewTicket, PermissionType.PostComment, PermissionType.ViewReports
                    }
                )
            },
            {
                RoleType.BusinessAnalyst,
                (
                    "Business Analyst",
                    "Creates and manages BRDs, user stories, and backlog items; can link requirements to tickets",
                    new List<PermissionType>
                    {
                        PermissionType.ViewProject, PermissionType.ViewSubProject, PermissionType.CreateBRD,
                        PermissionType.EditBRD, PermissionType.DeleteBRD, PermissionType.ViewBRD,
                        PermissionType.CreateUserStory, PermissionType.EditUserStory, PermissionType.DeleteUserStory,
                        PermissionType.ViewUserStory, PermissionType.ViewTicket, PermissionType.PostComment
                    }
                )
            },
            {
                RoleType.Viewer,
                (
                    "Viewer / Stakeholder",
                    "Read-only access to dashboards, reports, and ticket lists; cannot create or edit any content",
                    new List<PermissionType>
                    {
                        PermissionType.ViewProject, PermissionType.ViewSubProject, PermissionType.ViewSprint,
                        PermissionType.ViewTicket, PermissionType.ViewReports, PermissionType.ViewDashboards
                    }
                )
            },
            {
                RoleType.Guest,
                (
                    "Guest",
                    "Limited read-only access to a specific project via a shared invite link; no system account required",
                    new List<PermissionType>
                    {
                        PermissionType.ViewProject, PermissionType.ViewTicket, PermissionType.ViewReports
                    }
                )
            }
        };

        // Create roles if they don't exist
        foreach (var (roleType, (name, description, permissions)) in roles)
        {
            var existingRole = await _roleRepository.GetByTypeAsync((int)roleType);
            if (existingRole != null)
                continue;

            var role = new Role
            {
                Name = name,
                Description = description,
                RoleType = (int)roleType,
                IsSystemRole = true,
                IsActive = true
            };

            await _roleRepository.CreateAsync(role);
        }
    }
}
