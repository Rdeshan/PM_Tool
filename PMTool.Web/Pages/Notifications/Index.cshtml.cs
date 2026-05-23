using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Interfaces;
using System.Security.Claims;

namespace PMTool.Web.Pages.Notifications;

[Authorize]
public class IndexModel : PageModel
{
    private readonly INotificationService _notificationService;

    public IndexModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IActionResult> OnGetListAsync()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);
        return new JsonResult(notifications);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var success = await _notificationService.DeleteAsync(userId.Value, id);
        return new JsonResult(new { success });
    }

    private Guid? GetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : null;
    }
}
