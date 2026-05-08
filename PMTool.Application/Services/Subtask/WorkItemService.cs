using Microsoft.EntityFrameworkCore;
using PMTool.Application.DTOs.WorkItems;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Application.Services.Subtask
{
    // NEW: Work item service
    public class WorkItemService : IWorkItemService
    {
        // NEW: Database context
        private readonly AppDbContext _context;

        // NEW: Constructor
        public WorkItemService(AppDbContext context)
        {
            _context = context;
        }

        // NEW: Get all work items
        public async Task<List<WorkItemDto>> GetAllAsync()
        {
            return await _context.WorkItems
                .Include(w => w.WorkType)
                .Include(w => w.WorkStatus)
                .Include(w => w.Assignee)
                .Select(w => new WorkItemDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Description = w.Description,
                    WorkType = w.WorkType.Name,
                    Status = w.WorkStatus.Name,
                    Assignee = w.Assignee != null
                        ? w.Assignee.FirstName + " " + w.Assignee.LastName
                        : null,
                    Priority = w.Priority,
                    Progress = w.Progress,
                    SubTaskCount = w.SubTasks.Count
                })
                .ToListAsync();
        }

        // NEW: Get by ID
        public async Task<WorkItemDto?> GetByIdAsync(int id)
        {
            return await _context.WorkItems
                .Include(w => w.WorkType)
                .Include(w => w.WorkStatus)
                .Include(w => w.Assignee)
                .Where(w => w.Id == id)
                .Select(w => new WorkItemDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Description = w.Description,
                    WorkType = w.WorkType.Name,
                    Status = w.WorkStatus.Name,
                    Assignee = w.Assignee != null
                        ? w.Assignee.FirstName + " " + w.Assignee.LastName
                        : null,
                    Priority = w.Priority,
                    Progress = w.Progress,
                    SubTaskCount = w.SubTasks.Count
                })
                .FirstOrDefaultAsync();
        }

        // NEW: Create work item
        public async Task CreateAsync(CreateWorkItemDto dto)
        {
            var item = new WorkItem
            {
                Title = dto.Title,
                Description = dto.Description,
                WorkTypeId = dto.WorkTypeId,
                WorkStatusId = dto.WorkStatusId,
                AssigneeId = dto.AssigneeId,
                ProjectId = dto.ProjectId,
                SprintId = dto.SprintId,
                DueDate = dto.DueDate,
                Priority = dto.Priority
            };

            _context.WorkItems.Add(item);

            await _context.SaveChangesAsync();
        }

        // NEW: Update work item
        public async Task UpdateAsync(UpdateWorkItemDto dto)
        {
            var item = await _context.WorkItems.FindAsync(dto.Id);

            if (item == null)
                return;

            item.Title = dto.Title;
            item.Description = dto.Description;
            item.WorkStatusId = dto.WorkStatusId;
            item.AssigneeId = dto.AssigneeId;
            item.DueDate = dto.DueDate;
            item.Priority = dto.Priority;
            item.Progress = dto.Progress;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // NEW: Delete work item
        public async Task DeleteAsync(int id)
        {
            var item = await _context.WorkItems.FindAsync(id);

            if (item == null)
                return;

            _context.WorkItems.Remove(item);

            await _context.SaveChangesAsync();
        }
    }
}