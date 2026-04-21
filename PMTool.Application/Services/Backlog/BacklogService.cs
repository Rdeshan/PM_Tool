using PMTool.Application.DTOs.Backlog;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Backlog;

public class BacklogService : IBacklogService
{
    private readonly IProjectBacklogRepository _backlogRepository;
    private readonly IUserRepository _userRepository;

    public BacklogService(IProjectBacklogRepository backlogRepository, IUserRepository userRepository)
    {
        _backlogRepository = backlogRepository;
        _userRepository = userRepository;
    }

    public async Task<List<BacklogItemDTO>> GetBacklogItemsAsync(Guid projectId, Guid? productId, Guid? subProjectId, int? status)
    {
        var items = await _backlogRepository.GetByFilterAsync(projectId, productId, subProjectId, status);
        return items
            .OrderBy(i => i.Priority)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<BacklogItemDTO?> CreateBacklogItemAsync(CreateBacklogItemRequest request)
    {
        if (request.ProjectId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
        {
            return null;
        }

        var nextPriority = await _backlogRepository.GetNextPriorityAsync(request.ProjectId, request.ProductId);

        var item = new ProjectBacklog
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            ProductId = request.ProductId,
            SubProjectId = request.SubProjectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Type = request.Type,
            Status = request.Status,
            Priority = nextPriority,
            OwnerId = request.OwnerId,
            StartDate = request.StartDate,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _backlogRepository.CreateAsync(item);
        if (!created)
        {
            return null;
        }

        return MapToDto(item);
    }

    public async Task<BacklogItemDTO?> UpdateBacklogFieldAsync(UpdateBacklogFieldRequest request)
    {
        var item = await _backlogRepository.GetByIdAsync(request.ItemId);
        if (item == null)
        {
            return null;
        }

        switch (request.Field.ToLowerInvariant())
        {
            case "title":
                item.Title = request.Value.Trim();
                break;
            case "description":
                item.Description = request.Value.Trim();
                break;
            case "type":
                if (int.TryParse(request.Value, out var type))
                {
                    item.Type = type;
                }
                break;
            case "status":
                if (int.TryParse(request.Value, out var status))
                {
                    item.Status = status;
                }
                break;
            case "owner":
                item.OwnerId = Guid.TryParse(request.Value, out var ownerId) ? ownerId : null;
                break;
            case "priority":
                if (int.TryParse(request.Value, out var priority))
                {
                    item.Priority = priority;
                }
                break;
            case "startdate":
                item.StartDate = DateTime.TryParse(request.Value, out var startDate) ? startDate : null;
                break;
            case "duedate":
                item.DueDate = DateTime.TryParse(request.Value, out var dueDate) ? dueDate : null;
                break;
        }

        item.UpdatedAt = DateTime.UtcNow;

        var updated = await _backlogRepository.UpdateAsync(item);
        if (!updated)
        {
            return null;
        }

        if (item.OwnerId.HasValue)
        {
            item.Owner = await _userRepository.GetByIdAsync(item.OwnerId.Value);
        }

        return MapToDto(item);
    }

    public async Task<bool> ReorderItemsAsync(Guid projectId, Guid? productId, List<ReorderBacklogItemRequest> items)
    {
        var backlogItems = await _backlogRepository.GetByFilterAsync(projectId, productId, null, null);
        var map = items.ToDictionary(x => x.ItemId, x => x.Priority);

        foreach (var item in backlogItems)
        {
            if (map.TryGetValue(item.Id, out var newPriority))
            {
                item.Priority = newPriority;
                item.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await _backlogRepository.UpdateRangeAsync(backlogItems);
    }

    public Task<bool> DeleteItemAsync(Guid itemId)
    {
        return _backlogRepository.DeleteAsync(itemId);
    }

    private static BacklogItemDTO MapToDto(ProjectBacklog item)
    {
        return new BacklogItemDTO
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            ProductId = item.ProductId,
            OwnerId = item.OwnerId,
            OwnerName = item.Owner == null ? null : $"{item.Owner.FirstName} {item.Owner.LastName}".Trim(),
            Title = item.Title,
            Description = item.Description,
            Type = item.Type,
            TypeName = Enum.IsDefined(typeof(BacklogItemType), item.Type)
                ? ((BacklogItemType)item.Type).ToString()
                : "Unknown",
            Priority = item.Priority,
            Status = item.Status,
            StatusName = Enum.IsDefined(typeof(BacklogItemStatus), item.Status)
                ? ((BacklogItemStatus)item.Status).ToString()
                : "Unknown",
            StartDate = item.StartDate,
            DueDate = item.DueDate,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
