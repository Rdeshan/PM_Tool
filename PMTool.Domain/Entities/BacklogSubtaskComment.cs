namespace PMTool.Domain.Entities;

public class BacklogSubtaskComment
{
    public Guid Id { get; set; }
    public Guid SubtaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public BacklogSubtask? Subtask { get; set; }
    public User? Author { get; set; }
}
