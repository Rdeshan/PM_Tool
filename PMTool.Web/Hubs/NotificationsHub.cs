using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PMTool.Application.Interfaces;
using System.Security.Claims;

namespace PMTool.Web.Hubs;

[Authorize]
public class NotificationsHub : Hub
{
    private readonly INotificationService _notificationService;

    public NotificationsHub(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public override async Task OnConnectedAsync()
    {
        var rawUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(rawUserId, out var userId))
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            await Clients.Caller.SendAsync("notificationsSeed", notifications);
        }

        await base.OnConnectedAsync();
    }
}
