using PMTool.Application.DTOs.Notifications;

namespace PMTool.Application.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDTO>> GetUserNotificationsAsync(Guid userId);
    Task<NotificationDTO?> CreateAsync(Guid userId, string message, Guid? itemId = null);
    Task<bool> DeleteAsync(Guid userId, Guid notificationId);
}
