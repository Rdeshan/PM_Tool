namespace PMTool.Application.DTOs.Notifications;

public class NotificationDTO
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? ItemId { get; set; }
    public DateTime CreatedAt { get; set; }
}
