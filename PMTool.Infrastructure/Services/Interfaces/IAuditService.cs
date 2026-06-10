namespace PMTool.Infrastructure.Services.Interfaces;

public interface IAuditService
{
    Task LogAsync(Guid userId, string action, string entityType, string entityId,
        object? oldValue = null, object? newValue = null);
}
