using PMTool.Application.DTOs.Backlog;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Backlog;

public class ProductBacklogService : IProductBacklogService
{
    private readonly IProductBacklogRepository _backlogRepository;
    private readonly IBacklogSubtaskRepository _subtaskRepository;
    private readonly IUserRepository _userRepository;

    public ProductBacklogService(
        IProductBacklogRepository backlogRepository,
        IBacklogSubtaskRepository subtaskRepository,
        IUserRepository userRepository)
    {
        _backlogRepository = backlogRepository;
        _subtaskRepository = subtaskRepository;
        _userRepository = userRepository;
    }

    public async Task<ProductBacklogItemDTO?> GetItemByIdAsync(Guid itemId)
    {
        var item = await _backlogRepository.GetByIdAsync(itemId);
        return item == null ? null : MapToDto(item);
    }

    public async Task<List<ProductBacklogItemDTO>> GetBacklogItemsAsync(Guid productId, int? status)
    {
        var items = await _backlogRepository.GetByFilterAsync(productId, status);
        return items
            .OrderBy(i => i.Priority)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProductBacklogItemDTO?> CreateBacklogItemAsync(CreateProductBacklogItemRequest request)
    {
        if (request.ProductId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
        {
            return null;
        }

        var nextPriority = await _backlogRepository.GetNextPriorityAsync(request.ProductId);

        var item = new ProductBacklog
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            ParentBacklogItemId = request.ParentBacklogItemId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Type = request.Type,
            Status = request.Status == 0 ? (int)BacklogItemStatus.Draft : request.Status,
            Priority = nextPriority,
            OwnerId = request.OwnerId,
            StartDate = request.StartDate,
            DueDate = request.DueDate,
            StoryPoints = request.StoryPoints,
            SprintId = request.SprintId,
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

    public async Task<ProductBacklogItemDTO?> UpdateBacklogFieldAsync(UpdateProductBacklogFieldRequest request)
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
            case "parentbacklogitemid":
                item.ParentBacklogItemId = Guid.TryParse(request.Value, out var parentId) ? parentId : null;
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
            case "storypoints":
                if (int.TryParse(request.Value, out var storyPoints) && storyPoints >= 0)
                {
                    item.StoryPoints = storyPoints;
                }
                break;
            case "subproject":
                item.SubProjectId = Guid.TryParse(request.Value, out var subProjId) ? subProjId : null;
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

    public async Task<bool> ReorderItemsAsync(Guid productId, List<ReorderProductBacklogItemRequest> items)
    {
        var backlogItems = await _backlogRepository.GetByFilterAsync(productId, null);
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

    public async Task<BacklogSubtaskDto?> CreateSubtaskAsync(Guid parentId, CreateBacklogSubtaskDto request)
    {
        if (parentId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
        {
            return null;
        }

        var parent = await _backlogRepository.GetByIdAsync(parentId);
        if (parent == null)
        {
            return null;
        }

        var subtask = new BacklogSubtask
        {
            Id = Guid.NewGuid(),
            ParentId = parentId,
            ProductBacklogId = parentId,
            Title = request.Title.Trim(),
            Priority = request.Priority == 0 ? 3 : request.Priority,
            AssigneeId = request.AssigneeId,
            Status = request.Status == 0 ? 1 : request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _subtaskRepository.CreateAsync(subtask);
        if (!created)
        {
            return null;
        }

        if (subtask.AssigneeId.HasValue)
        {
            subtask.Assignee = await _userRepository.GetByIdAsync(subtask.AssigneeId.Value);
        }

        return MapSubtaskToDto(subtask);
    }

    public async Task<bool> UpdateSubtaskStatusAsync(Guid subtaskId, int status)
    {
        var subtask = await _subtaskRepository.GetByIdAsync(subtaskId);
        if (subtask == null)
        {
            return false;
        }

        subtask.Status = status;
        subtask.UpdatedAt = DateTime.UtcNow;

        return await _subtaskRepository.UpdateAsync(subtask);
    }

    public async Task<bool> UpdateSubtaskAsync(Guid subtaskId, CreateBacklogSubtaskDto request)
    {
        var subtask = await _subtaskRepository.GetByIdAsync(subtaskId);
        if (subtask == null) return false;

        subtask.Title = request.Title.Trim();
        subtask.Priority = request.Priority == 0 ? 3 : request.Priority;
        subtask.AssigneeId = request.AssigneeId;
        subtask.Status = request.Status == 0 ? 1 : request.Status;
        subtask.UpdatedAt = DateTime.UtcNow;

        return await _subtaskRepository.UpdateAsync(subtask);
    }

    public async Task<bool> DeleteSubtaskAsync(Guid subtaskId)
    {
        return await _subtaskRepository.DeleteAsync(subtaskId);
    }

    public Task<bool> DeleteItemAsync(Guid itemId)
    {
        return _backlogRepository.DeleteAsync(itemId);
    }

    private static ProductBacklogItemDTO MapToDto(ProductBacklog item)
    {
        return new ProductBacklogItemDTO
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ParentBacklogItemId = item.ParentBacklogItemId,
            OwnerId = item.OwnerId,
            OwnerName = item.Owner?.DisplayName ?? item.Owner?.FirstName,
            Title = item.Title,
            Description = item.Description,
            Type = item.Type,
            TypeName = GetWorkItemTypeName(item.Type),
            Priority = item.Priority,
            Status = item.Status,
            StatusName = GetStatusName(item.Status),
            StartDate = item.StartDate,
            DueDate = item.DueDate,
            StoryPoints = item.StoryPoints,
            SubProjectId = item.SubProjectId,
            SubProjectName = item.SubProject?.Name,
            SubProjectColor = item.SubProject?.ColorCode,
            SprintId = item.SprintId,
            Subtasks = item.Subtasks
                .OrderBy(s => s.CreatedAt)
                .Select(MapSubtaskToDto)
                .ToList(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    private static BacklogSubtaskDto MapSubtaskToDto(BacklogSubtask subtask)
    {
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
                1 => "Highest",
                2 => "High",
                3 => "Medium",
                4 => "Low",
                _ => "Medium"
            },
            AssigneeId = subtask.AssigneeId,
            AssigneeName = subtask.Assignee == null ? null : $"{subtask.Assignee.FirstName} {subtask.Assignee.LastName}".Trim(),
            Status = subtask.Status,
            StatusName = subtask.Status switch
            {
                1 => "To Do",
                2 => "In Progress",
                3 => "Done",
                _ => "To Do"
            },
            CreatedAt = subtask.CreatedAt,
            UpdatedAt = subtask.UpdatedAt
        };
    }

    private static string GetWorkItemTypeName(int type)
    {
        return ((BacklogItemType)type).ToString();
    }

    private static string GetStatusName(int status)
    {
        return ((BacklogItemStatus)status).ToString();
    }

    public List<BacklogItemTypeDTO> GetBacklogItemTypes()
    {
        return Enum.GetValues(typeof(BacklogItemType))
            .Cast<BacklogItemType>()
            .Select(x => new BacklogItemTypeDTO
            {
                Value = (int)x,
                Name = x.ToString(),
                Label = x switch
                {
                    BacklogItemType.BRD => "Business Requirement",
                    BacklogItemType.UserStory => "User Story",
                    BacklogItemType.UseCase => "Use Case",
                    BacklogItemType.Epic => "Epic",
                    BacklogItemType.ChangeRequest => "Change Request",
                    _ => x.ToString()
                }
            })
            .ToList();
    }

    public List<BacklogItemStatusDTO> GetBacklogItemStatuses()
    {
        return Enum.GetValues(typeof(BacklogItemStatus))
            .Cast<BacklogItemStatus>()
            .Select(x => new BacklogItemStatusDTO
            {
                Value = (int)x,
                Name = x.ToString(),
                Label = x switch
                {
                    BacklogItemStatus.Draft => "Draft",
                    BacklogItemStatus.Approved => "Approved",
                    BacklogItemStatus.InProgress => "In Progress",
                    BacklogItemStatus.Done => "Done",
                    _ => x.ToString()
                }
            })
            .ToList();
    }
}
