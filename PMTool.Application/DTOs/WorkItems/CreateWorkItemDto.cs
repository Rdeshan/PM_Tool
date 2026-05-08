using System.ComponentModel.DataAnnotations;

namespace PMTool.Application.DTOs.WorkItems
{
    // NEW: Create work item DTO
    public class CreateWorkItemDto
    {
        // NEW: Task title
        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        // NEW: Task description
        public string? Description { get; set; }

        // NEW: Work type
        public int WorkTypeId { get; set; }

        // NEW: Status
        public int WorkStatusId { get; set; }

        // NEW: Assignee
        public Guid? AssigneeId { get; set; }

        // NEW: Project
        public Guid ProjectId { get; set; }

        // NEW: Sprint
        public Guid? SprintId { get; set; }

        // NEW: Due date
        public DateTime? DueDate { get; set; }

        // NEW: Priority
        public int Priority { get; set; }
    }
}