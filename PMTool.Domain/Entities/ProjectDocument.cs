namespace PMTool.Domain.Entities;

public class ProjectDocument
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public int DocumentType { get; set; } = 3; // 1=BRD, 2=SRS, 3=Other
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid SubmittedByUserId { get; set; }
    public DateTime SubmittedAt { get; set; }

    public Project Project { get; set; } = null!;
    public User SubmittedByUser { get; set; } = null!;
}
