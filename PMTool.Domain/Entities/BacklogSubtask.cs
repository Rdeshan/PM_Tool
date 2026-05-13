using System.ComponentModel.DataAnnotations;

namespace PMTool.Domain.Entities
{
    public class BacklogSubtask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParentId { get; set; } // API/display parent id for either backlog table
        public Guid? ProjectBacklogId { get; set; }
        public Guid? ProductBacklogId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; } // 1: Highest, 2: High, 3: Medium, 4: Low
        public Guid? AssigneeId { get; set; }
        public int Status { get; set; } // 1: To Do, 2: In Progress, 3: Done
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ProjectBacklog? ProjectBacklog { get; set; }
        public ProductBacklog? ProductBacklog { get; set; }
        public User? Assignee { get; set; }
    }
}
