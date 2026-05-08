namespace PMTool.Application.DTOs.WorkItems
{
    // NEW: Display DTO
    public class WorkItemDto
    {
        // NEW: Work item ID
        public int Id { get; set; }

        // NEW: Task title
        public string Title { get; set; } = string.Empty;

        // NEW: Description
        public string? Description { get; set; }

        // NEW: Type name
        public string WorkType { get; set; } = string.Empty;

        // NEW: Status name
        public string Status { get; set; } = string.Empty;

        // NEW: Assigned user
        public string? Assignee { get; set; }

        // NEW: Priority
        public int Priority { get; set; }

        // NEW: Progress %
        public int Progress { get; set; }

        // NEW: Subtask count
        public int SubTaskCount { get; set; }
    }
}