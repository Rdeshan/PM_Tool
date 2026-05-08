using System.ComponentModel.DataAnnotations;

namespace PMTool.Domain.Entities
{
    // NEW: Comments for tasks
    public class WorkComment
    {
        public int Id { get; set; }

        // NEW: Comment content
        [Required]
        [MaxLength(2000)]
        public string Comment { get; set; } = string.Empty;

        // NEW: Parent work item
        public int WorkItemId { get; set; }

        // NEW: Navigation property
        public WorkItem WorkItem { get; set; } = null!;

        // NEW: Comment creator
        public Guid UserId { get; set; }

        // NEW: Navigation property
        public User User { get; set; } = null!;

        // NEW: Comment created date
        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;
    }
}