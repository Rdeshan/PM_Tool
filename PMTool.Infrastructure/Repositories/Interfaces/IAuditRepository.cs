using PMTool.Domain.Entities;
using PMTool.Infrastructure.DTOs.Audit;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IAuditRepository
{
    Task AddAsync(AuditLog auditLog);
    Task<(IEnumerable<AuditLogDto> Items, int TotalCount)> QueryAsync(AuditQueryRequest request);
    Task<IEnumerable<AuditLogDto>> GetByEntityAsync(string entityType, string entityId, int limit = 20);
}
