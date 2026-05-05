using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        return sprints.Select(MapToDto).ToList();
    }

    public async Task<SprintDTO> CreateSprintAsync(CreateSprintRequest request)
    {
        var sprint = new Domain.Entities.Sprint
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Goal = request.Goal,
            Status = 1 // Draft
        };

        var created = await _sprintRepository.AddAsync(sprint);
        return MapToDto(created);
    }

    public async Task<bool> UpdateSprintAsync(SprintDTO dto)
    {
        var sprint = await _sprintRepository.GetByIdAsync(dto.Id);
        if (sprint == null) return false;

        sprint.Name = dto.Name;
        sprint.StartDate = dto.StartDate;
        sprint.EndDate = dto.EndDate;
        sprint.Goal = dto.Goal;
        sprint.Status = dto.Status;

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
        item.SprintId = sprintId;
        await _backlogRepository.UpdateAsync(item);

        // Log scope change
        if (oldSprintId.HasValue)
        {
            await _sprintRepository.AddScopeChangeAsync(new SprintScopeChange
            {
                Id = Guid.NewGuid(),
                SprintId = oldSprintId.Value,
                BacklogItemId = itemId,
                ChangeType = "Removed",
                ChangedById = userId
            });
        }

        if (sprintId.HasValue)
        {
            await _sprintRepository.AddScopeChangeAsync(new SprintScopeChange
            {
                Id = Guid.NewGuid(),
                SprintId = sprintId.Value,
                BacklogItemId = itemId,
                ChangeType = "Added",
                ChangedById = userId
            });
        }

        return true;
    }

    private SprintDTO MapToDto(Domain.Entities.Sprint sprint)
    {
        return new SprintDTO
        {
            Id = sprint.Id,
            ProductId = sprint.ProductId,
            Name = sprint.Name,
            StartDate = sprint.StartDate,
            EndDate = sprint.EndDate,
            Goal = sprint.Goal,
            Status = sprint.Status,
            TotalPoints = sprint.BacklogItems?.Sum(x => x.StoryPoints) ?? 0,
            ItemCount = sprint.BacklogItems?.Count ?? 0
        };
    }
}
