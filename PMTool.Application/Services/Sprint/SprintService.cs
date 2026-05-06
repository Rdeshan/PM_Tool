using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMTool.Application.DTOs.Backlog;
using PMTool.Application.DTOs.Sprint;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Sprint;

public class SprintService : ISprintService
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IProductBacklogRepository _backlogRepository;

    public SprintService(ISprintRepository sprintRepository, IProductBacklogRepository backlogRepository)
    {
        _sprintRepository = sprintRepository;
        _backlogRepository = backlogRepository;
    }

    public async Task<List<SprintDTO>> GetSprintsByProductAsync(Guid productId)
    {
        var sprints = await _sprintRepository.GetByProductIdAsync(productId);
        return sprints.Select(s => MapToDto(s, includeItems: false)).ToList();
    }

    public async Task<SprintDTO?> GetActiveSprintAsync(Guid productId)
    {
        var sprint = await _sprintRepository.GetActiveByProductIdAsync(productId);
        return sprint == null ? null : MapToDto(sprint, includeItems: true);
    }

    public async Task<SprintDTO> CreateSprintAsync(CreateSprintRequest request)
    {
        var sprint = new Domain.Entities.Sprint
        {
            Id        = Guid.NewGuid(),
            ProductId = request.ProductId,
            Name      = request.Name,
            StartDate = request.StartDate,
            EndDate   = request.EndDate,
            Goal      = request.Goal,
            Status    = 1 // Draft
        };

        var created = await _sprintRepository.AddAsync(sprint);
        return MapToDto(created, includeItems: false);
    }

    public async Task<bool> UpdateSprintAsync(SprintDTO dto)
    {
        var sprint = await _sprintRepository.GetByIdAsync(dto.Id);
        if (sprint == null) return false;

        sprint.Name      = dto.Name;
        sprint.StartDate = dto.StartDate;
        sprint.EndDate   = dto.EndDate;
        sprint.Goal      = dto.Goal;
        sprint.Status    = dto.Status;

        await _sprintRepository.UpdateAsync(sprint);
        return true;
    }

    public async Task<bool> DeleteSprintAsync(Guid id)
    {
        var sprint = await _sprintRepository.GetByIdAsync(id);
        if (sprint == null) return false;

        // Move all items back to backlog
        foreach (var item in sprint.BacklogItems.ToList())
        {
            item.SprintId = null;
            await _backlogRepository.UpdateAsync(item);
        }

        await _sprintRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> MoveToSprintAsync(Guid itemId, Guid? sprintId, Guid userId)
    {
        var item = await _backlogRepository.GetByIdAsync(itemId);
        if (item == null) return false;

        var oldSprintId = item.SprintId;
        item.SprintId   = sprintId;
        await _backlogRepository.UpdateAsync(item);

        // Log scope change
        if (oldSprintId.HasValue)
        {
            await _sprintRepository.AddScopeChangeAsync(new SprintScopeChange
            {
                Id            = Guid.NewGuid(),
                SprintId      = oldSprintId.Value,
                BacklogItemId = itemId,
                ChangeType    = "Removed",
                ChangedById   = userId
            });
        }

        if (sprintId.HasValue)
        {
            await _sprintRepository.AddScopeChangeAsync(new SprintScopeChange
            {
                Id            = Guid.NewGuid(),
                SprintId      = sprintId.Value,
                BacklogItemId = itemId,
                ChangeType    = "Added",
                ChangedById   = userId
            });
        }

        return true;
    }

    // ── Sprint lifecycle ──────────────────────────────────────────────────────

    public async Task<SprintDTO?> StartSprintAsync(Guid sprintId, DateTime startDate, DateTime endDate)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId);
        if (sprint == null) return null;

        sprint.Status    = 2; // Active
        sprint.StartDate = startDate;
        sprint.EndDate   = endDate;

        await _sprintRepository.UpdateAsync(sprint);

        // Reload with items so the Board URL can be returned together with full data
        var refreshed = await _sprintRepository.GetByIdAsync(sprintId);
        return refreshed == null ? null : MapToDto(refreshed, includeItems: false);
    }

    public async Task<bool> CompleteSprintAsync(Guid sprintId)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId);
        if (sprint == null) return false;

        sprint.Status = 3; // Completed
        await _sprintRepository.UpdateAsync(sprint);
        return true;
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    private static SprintDTO MapToDto(Domain.Entities.Sprint sprint, bool includeItems)
    {
        var dto = new SprintDTO
        {
            Id          = sprint.Id,
            ProductId   = sprint.ProductId,
            Name        = sprint.Name,
            StartDate   = sprint.StartDate,
            EndDate     = sprint.EndDate,
            Goal        = sprint.Goal,
            Status      = sprint.Status,
            TotalPoints = sprint.BacklogItems?.Sum(x => x.StoryPoints) ?? 0,
            ItemCount   = sprint.BacklogItems?.Count ?? 0
        };

        if (includeItems && sprint.BacklogItems != null)
        {
            dto.BacklogItems = sprint.BacklogItems.Select(MapItemToDto).ToList();
        }

        return dto;
    }

    private static ProductBacklogItemDTO MapItemToDto(ProductBacklog item)
    {
        var ownerName = item.Owner != null
            ? (item.Owner.DisplayName ?? $"{item.Owner.FirstName} {item.Owner.LastName}").Trim()
            : null;

        return new ProductBacklogItemDTO
        {
            Id           = item.Id,
            ProductId    = item.ProductId,
            Key          = $"PB-{item.Id.ToString("N")[..8].ToUpper()}",
            OwnerId      = item.OwnerId,
            OwnerName    = ownerName,
            Title        = item.Title,
            Description  = item.Description,
            Type         = item.Type,
            Priority     = item.Priority,
            Status       = item.Status,
            StartDate    = item.StartDate,
            DueDate      = item.DueDate,
            StoryPoints  = item.StoryPoints,
            SubProjectId = item.SubProjectId,
            SprintId     = item.SprintId,
            CreatedAt    = item.CreatedAt,
            UpdatedAt    = item.UpdatedAt
        };
    }
}
