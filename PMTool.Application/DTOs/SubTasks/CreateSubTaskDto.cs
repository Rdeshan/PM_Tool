using System.ComponentModel.DataAnnotations;

namespace PMTool.Application.DTOs.SubTasks
{
    // NEW: Create subtask DTO
    public class CreateSubTaskDto
    {
        // NEW: Title
        [Required]
        public string Title { get; set; } = string.Empty;

        // NEW: Parent work item
        public int WorkItemId { get; set; }

        // NEW: Assignee
        public Guid? AssigneeId { get; set; }
    }
}