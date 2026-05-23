using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(Guid userId);
    Task<Notification?> AddAsync(Notification notification);
    Task<bool> DeleteAsync(Notification notification);
    Task<Notification?> GetByIdAsync(Guid id);
}
