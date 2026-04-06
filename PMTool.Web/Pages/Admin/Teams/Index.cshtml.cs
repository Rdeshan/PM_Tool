using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Team;
using PMTool.Application.DTOs.User;
using PMTool.Application.Interfaces;
using PMTool.Application.Services.Team;

namespace PMTool.Web.Pages.Admin.Teams;

[Authorize(Roles = "Administrator,Project Manager")]
public class IndexModel : PageModel
{
    private readonly ITeamService _teamService;
    private readonly IUserAdminService _userAdminService;

    public IEnumerable<TeamDTO> Teams { get; set; } = new List<TeamDTO>();
    public Guid? SelectedTeamId { get; set; }
    public IEnumerable<UserDTO> TeamMembers { get; set; } = new List<UserDTO>();
    public IEnumerable<UserDTO> AvailableUsers { get; set; } = new List<UserDTO>();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public CreateTeamRequest Input { get; set; } = new();

    public IndexModel(ITeamService teamService, IUserAdminService userAdminService)
    {
        _teamService = teamService;
        _userAdminService = userAdminService;
    }

    public async Task OnGetAsync(Guid? teamId = null)
    {
        SuccessMessage = TempData["SuccessMessage"]?.ToString();
        ErrorMessage = TempData["ErrorMessage"]?.ToString();

        Teams = await _teamService.GetAllTeamsAsync();
        AvailableUsers = await _userAdminService.GetActiveUsersAsync();

        if (teamId.HasValue && teamId != Guid.Empty)
        {
            SelectedTeamId = teamId;
            TeamMembers = await _userAdminService.GetUsersByTeamAsync(teamId.Value);
        }
    }

    public async Task<IActionResult> OnPostCreateTeamAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fill in all required fields.";
            await OnGetAsync();
            return Page();
        }

        var result = await _teamService.CreateTeamAsync(Input);
        if (result)
        {
            TempData["SuccessMessage"] = $"Team '{Input.Name}' created successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to create team.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddMemberAsync(Guid userId, Guid teamId)
    {
        var result = await _userAdminService.AddUserToTeamAsync(userId, teamId);
        if (result)
        {
            TempData["SuccessMessage"] = "User added to team successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to add user to team.";
        }

        return RedirectToPage(new { teamId });
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(Guid userId, Guid teamId)
    {
        var result = await _userAdminService.RemoveUserFromTeamAsync(userId, teamId);
        if (result)
        {
            TempData["SuccessMessage"] = "User removed from team successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to remove user from team.";
        }

        return RedirectToPage(new { teamId });
    }
}
