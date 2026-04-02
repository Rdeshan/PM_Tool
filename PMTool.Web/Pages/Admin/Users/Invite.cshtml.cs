using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.User;
using PMTool.Application.Services.User;
using PMTool.Application.Services.RBAC;
using PMTool.Application.Services.Team;
using System.Security.Claims;

namespace PMTool.Web.Pages.Admin.Users;

[Authorize(Roles = "Administrator")]
public class InviteModel : PageModel
{
    private readonly IUserAdminService _userAdminService;
    private readonly IRoleService _roleService;
    private readonly ITeamService _teamService;

    [BindProperty]
    public InviteUserRequest Input { get; set; } = new();

    public List<dynamic> AvailableRoles { get; set; } = new();
    public List<dynamic> AvailableTeams { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public InviteModel(IUserAdminService userAdminService, IRoleService roleService, ITeamService teamService)
    {
        _userAdminService = userAdminService;
        _roleService = roleService;
        _teamService = teamService;
    }

    public async Task OnGetAsync()
    {
        AvailableRoles = (await _roleService.GetAllRolesAsync())
            .Select(r => new { r.Id, r.Name, r.Description })
            .Cast<dynamic>()
            .ToList();

        AvailableTeams = (await _teamService.GetActiveTeamsAsync())
            .Select(t => new { t.Id, t.Name, t.MemberCount })
            .Cast<dynamic>()
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            ErrorMessage = "Please fill in all required fields correctly.";
            return Page();
        }

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _userAdminService.InviteUserAsync(Input, userId);

        if (!result)
        {
            await OnGetAsync();
            ErrorMessage = "Failed to invite user. Email may already exist or an error occurred.";
            return Page();
        }

        SuccessMessage = $"Invitation sent to {Input.Email}. User will receive an email with account setup instructions.";
        Input = new InviteUserRequest();
        await OnGetAsync();
        return Page();
    }
}
