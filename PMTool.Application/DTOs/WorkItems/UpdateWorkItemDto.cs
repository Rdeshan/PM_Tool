namespace PMTool.Application.DTOs.WorkItems
{
    // NEW: Update DTO
    public class UpdateWorkItemDto
    {
        // NEW: Work item ID
        public int Id { get; set; }

        // NEW: Title
        public string Title { get; set; } = string.Empty;

        // NEW: Description
        public string? Description { get; set; }

        // NEW: Status
        public int WorkStatusId { get; set; }

        // NEW: Assignee
        public Guid? AssigneeId { get; set; }

        // NEW: Due date
        public DateTime? DueDate { get; set; }

        // NEW: Priority
        public int Priority { get; set; }

        // NEW: Progress
        public int Progress { get; set; }
    }
}