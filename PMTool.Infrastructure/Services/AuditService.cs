using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services.Interfaces;

namespace PMTool.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AuditService(
        IAuditRepository repository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(Guid userId, string action, string entityType, string entityId,
        object? oldValue = null, object? newValue = null)
    {
        try
        {
            // If no explicit userId, read it from the cookie claims
            var resolvedUserId = userId == Guid.Empty ? ResolveUserIdFromContext() : userId;

            // Skip the write entirely if we still cannot identify the user
            // (e.g. background operations with no HTTP context)
            if (resolvedUserId == Guid.Empty)
            {
                _logger.LogWarning("Audit log skipped — could not resolve user for action {Action} on {EntityType}/{EntityId}", action, entityType, entityId);
                return;
            }

            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = resolvedUserId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValue = oldValue is null ? null : JsonSerializer.Serialize(oldValue, _jsonOptions),
                NewValue = newValue is null ? null : JsonSerializer.Serialize(newValue, _jsonOptions),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit log write failed for action {Action} on {EntityType}/{EntityId}", action, entityType, entityId);
        }
    }

    private Guid ResolveUserIdFromContext()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }
}
