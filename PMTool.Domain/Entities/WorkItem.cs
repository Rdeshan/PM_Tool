using System.ComponentModel.DataAnnotations;

namespace PMTool.Domain.Entities
{
    // NEW: Main task entity
    public class WorkItem
    {
        public int Id { get; set; }

        // NEW: Work item title
        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        // NEW: Description
        [MaxLength(3000)]
        public string? Description { get; set; }

        // NEW: Work type relation
        public int WorkTypeId { get; set; }

        // NEW: Navigation property
        public WorkType WorkType { get; set; } = null!;

        // NEW: Status relation
        public int WorkStatusId { get; set; }

        // NEW: Navigation property
        public WorkStatus WorkStatus { get; set; } = null!;

        // NEW: Assigned user
        public Guid? AssigneeId { get; set; }

        // NEW: Navigation property
        public User? Assignee { get; set; }

        // NEW: Parent project
        public Guid ProjectId { get; set; }

        // NEW: Navigation property
        public Project Project { get; set; } = null!;

        // NEW: Optional sprint
        public Guid? SprintId { get; set; }

        // NEW: Navigation property
        public Sprint? Sprint { get; set; }

        // NEW: Due date
        public DateTime? DueDate { get; set; }

        // NEW: Priority
        public int Priority { get; set; } = 1;

        // NEW: Progress %
        public int Progress { get; set; } = 0;

        // NEW: Created date
        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // NEW: Updated date
        public DateTime UpdatedAt { get; set; }
            = DateTime.UtcNow;

        // NEW: Subtasks
        public ICollection<SubTask> SubTasks { get; set; }
            = new List<SubTask>();

        // NEW: Comments
        public ICollection<WorkComment> Comments { get; set; }
            = new List<WorkComment>();
    }
}