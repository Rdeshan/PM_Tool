using PMTool.Application.DTOs.Backlog;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Backlog;

public class WorkTypeService : IWorkTypeService
{
    private readonly IWorkTypeRepository _workTypeRepository;

    public WorkTypeService(IWorkTypeRepository workTypeRepository)
    {
        _workTypeRepository = workTypeRepository;
    }

    public async Task<List<WorkTypeDTO>> GetCustomWorkTypesAsync()
    {
        var workTypes = await _workTypeRepository.GetCustomAsync();
        return workTypes
            .Select(x => new WorkTypeDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IconClass = x.Key
            })
            .ToList();
    }

    public async Task<WorkTypeDTO?> CreateWorkTypeAsync(CreateWorkTypeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.IconClass))
        {
            return null;
        }

        if (await _workTypeRepository.ExistsByNameAsync(request.Name.Trim()))
        {
            return null;
        }

        var workType = new WorkType
        {
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Key = request.IconClass.Trim(),
            CreatedDate = DateTime.UtcNow,
            IsDefault = false
        };

        var created = await _workTypeRepository.CreateAsync(workType);
        if (created == null)
        {
            return null;
        }

        return new WorkTypeDTO
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            IconClass = created.Key
        };
    }
}
