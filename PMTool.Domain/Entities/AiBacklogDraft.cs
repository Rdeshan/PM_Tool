namespace PMTool.Domain.Entities;

public class AiBacklogDraft
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public Guid ProjectId { get; set; }
    public string SourceFileName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid GeneratedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }

    public Product? Product { get; set; }
    public Project? Project { get; set; }
    public User? GeneratedBy { get; set; }
    public User? ApprovedBy { get; set; }
    public ICollection<AiBacklogDraftItem> Items { get; set; } = new List<AiBacklogDraftItem>();
}
