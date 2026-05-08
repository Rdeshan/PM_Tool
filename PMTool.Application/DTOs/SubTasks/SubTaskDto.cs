namespace PMTool.Application.DTOs.SubTasks
{
    // NEW: Display subtask DTO
    public class SubTaskDto
    {
        // NEW: Subtask ID
        public int Id { get; set; }

        // NEW: Title
        public string Title { get; set; } = string.Empty;

        // NEW: Completion
        public bool IsCompleted { get; set; }

        // NEW: Assignee
        public string? Assignee { get; set; }
    }
}
