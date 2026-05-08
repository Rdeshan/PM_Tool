using System.ComponentModel.DataAnnotations;

namespace PMTool.Domain.Entities
{
    // NEW: Work item status entity
    public class WorkStatus
    {
        public int Id { get; set; }

        // NEW: Status name
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // NEW: UI color
        [MaxLength(20)]
        public string Color { get; set; } = "gray";

        // NEW: Completion tracking
        public bool IsCompleted { get; set; } = false;

        // NEW: Navigation property
        public ICollection<WorkItem> WorkItems { get; set; }
            = new List<WorkItem>();
    }
}