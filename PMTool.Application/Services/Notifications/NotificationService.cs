using PMTool.Application.DTOs.Notifications;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDTO>> GetUserNotificationsAsync(Guid userId)
    {
        var rawNotifications = await _notificationRepository.GetByUserIdAsync(userId);
        var dtos = new List<NotificationDTO>();
        // Wait, NotificationRepository does not have access to ProductBacklog.
        // It's better to fetch via AppDbContext, but NotificationService uses INotificationRepository.
        // I will just use INotificationRepository and since we cannot easily join here without AppDbContext, 
        // I need to reconsider how to fetch it.

    public async Task<NotificationDTO?> CreateAsync(Guid userId, string message, Guid? itemId = null)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Message = message.Trim(),
            ItemId = itemId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _notificationRepository.AddAsync(notification);
        return created == null ? null : MapToDto(created);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid notificationId)
    {
        if (userId == Guid.Empty || notificationId == Guid.Empty)
        {
            return false;
        }

        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null || notification.UserId != userId)
        {
            return false;
        }

        return await _notificationRepository.DeleteAsync(notification);
    }

    private static NotificationDTO MapToDto(Notification notification)
    {
        return new NotificationDTO
        {
            Id = notification.Id,
            Message = notification.Message,
            ItemId = notification.ItemId,
            CreatedAt = notification.CreatedAt
        };
    }
}
