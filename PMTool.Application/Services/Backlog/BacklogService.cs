using PMTool.Application.DTOs.Backlog;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services.Interfaces;

namespace PMTool.Application.Services.Backlog;

public class BacklogService : IBacklogService
{
    private readonly IProjectBacklogRepository _backlogRepository;
    private readonly IBacklogSubtaskRepository _subtaskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;

    public BacklogService(
        IProjectBacklogRepository backlogRepository,
        IBacklogSubtaskRepository subtaskRepository,
        IUserRepository userRepository,
        IAuditService auditService)
    {
        _backlogRepository = backlogRepository;
        _subtaskRepository = subtaskRepository;
        _userRepository = userRepository;
        _auditService = auditService;
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
            return null;

        var nextPriority = await _backlogRepository.GetNextPriorityAsync(request.ProjectId, request.ProductId);

        var item = new ProjectBacklog
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            ProductId = request.ProductId,
            SubProjectId = request.SubProjectId,
            ParentBacklogItemId = request.ParentBacklogItemId,
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
            return null;

        await _auditService.LogAsync(Guid.Empty, "ProjectBacklog.Created", "ProjectBacklog", item.Id.ToString(),
            newValue: new { item.Title, type = ((BacklogItemType)item.Type).ToString(), projectId = item.ProjectId });

        return MapToDto(item);
    }

    public async Task<BacklogItemDTO?> UpdateBacklogFieldAsync(UpdateBacklogFieldRequest request)
    {
        var item = await _backlogRepository.GetByIdAsync(request.ItemId);
        if (item == null)
            return null;

        // Capture old value before mutation
        var oldValue = GetFieldValue(item, request.Field);

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
                    item.Type = type;
                break;
            case "status":
                if (int.TryParse(request.Value, out var status))
                    item.Status = status;
                break;
            case "owner":
                item.OwnerId = Guid.TryParse(request.Value, out var ownerId) ? ownerId : null;
                break;
            case "parentbacklogitemid":
                item.ParentBacklogItemId = Guid.TryParse(request.Value, out var parentId) ? parentId : null;
                break;
            case "priority":
                if (int.TryParse(request.Value, out var priority))
                    item.Priority = priority;
                break;
            case "startdate":
                item.StartDate = DateTime.TryParse(request.Value, out var startDate) ? startDate : null;
                break;
            case "duedate":
                item.DueDate = DateTime.TryParse(request.Value, out var dueDate) ? dueDate : null;
                break;
            case "storypoints":
                item.StoryPoints = int.TryParse(request.Value, out var sp) ? sp : null;
                break;
        }

        item.UpdatedAt = DateTime.UtcNow;

        var updated = await _backlogRepository.UpdateAsync(item);
        if (!updated)
            return null;

        await _auditService.LogAsync(Guid.Empty, "ProjectBacklog.FieldChanged", "ProjectBacklog", item.Id.ToString(),
            oldValue: new { field = request.Field, value = oldValue },
            newValue: new { field = request.Field, value = request.Value });

        if (item.OwnerId.HasValue)
            item.Owner = await _userRepository.GetByIdAsync(item.OwnerId.Value);

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

    public async Task<BacklogItemDTO?> GetBacklogItemAsync(Guid itemId)
    {
        var item = await _backlogRepository.GetByIdAsync(itemId);
        return item == null ? null : MapToDto(item);
    }

    public async Task<BacklogSubtaskDto?> CreateSubtaskAsync(Guid parentId, CreateBacklogSubtaskDto request)
    {
        if (parentId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
            return null;

        var parent = await _backlogRepository.GetByIdAsync(parentId);
        if (parent == null)
            return null;

        var subtask = new BacklogSubtask
        {
            Id = Guid.NewGuid(),
            ParentId = parentId,
            ProjectBacklogId = parentId,
            Title = request.Title.Trim(),
            Priority = request.Priority == 0 ? 3 : request.Priority,
            AssigneeId = request.AssigneeId,
            Status = request.Status == 0 ? 1 : request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _subtaskRepository.CreateAsync(subtask);
        if (!created) return null;

        if (subtask.AssigneeId.HasValue)
            subtask.Assignee = await _userRepository.GetByIdAsync(subtask.AssigneeId.Value);

        return new BacklogSubtaskDto
        {
            Id = subtask.Id,
            ParentId = subtask.ParentId,
            ProductBacklogId = subtask.ProductBacklogId,
            ProjectBacklogId = subtask.ProjectBacklogId,
            Title = subtask.Title,
            Priority = subtask.Priority,
            PriorityName = subtask.Priority switch
            {
                1 => "Highest", 2 => "High", 3 => "Medium", 4 => "Low", _ => "Unknown"
            },
            AssigneeId = subtask.AssigneeId,
            AssigneeName = subtask.Assignee == null ? null : $"{subtask.Assignee.FirstName} {subtask.Assignee.LastName}".Trim(),
            Status = subtask.Status,
            StatusName = subtask.Status switch
            {
                1 => "To Do", 2 => "In Progress", 3 => "Done", _ => "Unknown"
            },
            CreatedAt = subtask.CreatedAt,
            UpdatedAt = subtask.UpdatedAt
        };
    }

    public async Task<bool> UpdateSubtaskStatusAsync(Guid subtaskId, int status)
    {
        var subtask = await _subtaskRepository.GetByIdAsync(subtaskId);
        if (subtask == null) return false;

        subtask.Status = status;
        subtask.UpdatedAt = DateTime.UtcNow;

        return await _subtaskRepository.UpdateAsync(subtask);
    }

    public async Task<bool> DeleteItemAsync(Guid itemId)
    {
        var item = await _backlogRepository.GetByIdAsync(itemId);
        var result = await _backlogRepository.DeleteAsync(itemId);
        if (result && item != null)
            await _auditService.LogAsync(Guid.Empty, "ProjectBacklog.Deleted", "ProjectBacklog", itemId.ToString(),
                oldValue: new { item.Title, projectId = item.ProjectId });
        return result;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string? GetFieldValue(ProjectBacklog item, string field) => field.ToLowerInvariant() switch
    {
        "title"              => item.Title,
        "description"        => item.Description,
        "type"               => item.Type.ToString(),
        "status"             => item.Status.ToString(),
        "owner"              => item.OwnerId?.ToString(),
        "parentbacklogitemid"=> item.ParentBacklogItemId?.ToString(),
        "priority"           => item.Priority.ToString(),
        "startdate"          => item.StartDate?.ToString("O"),
        "duedate"            => item.DueDate?.ToString("O"),
        "storypoints"        => item.StoryPoints?.ToString(),
        _                    => null
    };

    private static BacklogItemDTO MapToDto(ProjectBacklog item)
    {
        return new BacklogItemDTO
        {
            Id = item.Id,
            ProjectId = item.ProjectId,
            ProductId = item.ProductId,
            SubProjectId = item.SubProjectId,
            ParentBacklogItemId = item.ParentBacklogItemId,
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
            UpdatedAt = item.UpdatedAt,
            StoryPoints = item.StoryPoints,
            ParentBacklogItemTitle = item.ParentBacklogItem?.Title,
            Subtasks = item.Subtasks.Select(s => new BacklogSubtaskDto
            {
                Id = s.Id,
                ParentId = s.ParentId,
                ProductBacklogId = s.ProductBacklogId,
                ProjectBacklogId = s.ProjectBacklogId,
                Title = s.Title,
                Priority = s.Priority,
                PriorityName = s.Priority switch
                {
                    1 => "Highest", 2 => "High", 3 => "Medium", 4 => "Low", _ => "Unknown"
                },
                AssigneeId = s.AssigneeId,
                AssigneeName = s.Assignee == null ? null : $"{s.Assignee.FirstName} {s.Assignee.LastName}".Trim(),
                Status = s.Status,
                StatusName = s.Status switch
                {
                    1 => "To Do", 2 => "In Progress", 3 => "Done", _ => "Unknown"
                },
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList()
        };
    }
}
