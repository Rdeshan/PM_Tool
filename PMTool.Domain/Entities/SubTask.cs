using System.ComponentModel.DataAnnotations;

namespace PMTool.Domain.Entities
{
    // NEW: Sub task entity
    public class SubTask
    {
        public int Id { get; set; }

        // NEW: Subtask title
        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        // NEW: Completion status
        public bool IsCompleted { get; set; } = false;

        // NEW: Parent work item
        public int WorkItemId { get; set; }

        // NEW: Navigation property
        public WorkItem WorkItem { get; set; } = null!;

        // NEW: Assigned user
        public Guid? AssigneeId { get; set; }

        // NEW: Navigation property
        public User? Assignee { get; set; }

        // NEW: Created date
        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}