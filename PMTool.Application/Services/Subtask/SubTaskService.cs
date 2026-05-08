using PMTool.Application.DTOs.SubTasks;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Application.Services.Subtask
{
    // NEW: Subtask service
    public class SubTaskService : ISubTaskService
    {
        // NEW: Database context
        private readonly AppDbContext _context;

        // NEW: Constructor
        public SubTaskService(AppDbContext context)
        {
            _context = context;
        }

        // NEW: Create subtask
        public async Task CreateAsync(CreateSubTaskDto dto)
        {
            var subTask = new SubTask
            {
                Title = dto.Title,
                WorkItemId = dto.WorkItemId,
                AssigneeId = dto.AssigneeId
            };

            _context.SubTasks.Add(subTask);

            await _context.SaveChangesAsync();
        }

        // NEW: Toggle complete
        public async Task ToggleCompleteAsync(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);

            if (subTask == null)
                return;

            subTask.IsCompleted = !subTask.IsCompleted;

            await _context.SaveChangesAsync();
        }

        // NEW: Delete subtask
        public async Task DeleteAsync(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);

            if (subTask == null)
                return;

            _context.SubTasks.Remove(subTask);

            await _context.SaveChangesAsync();
        }
    }
}