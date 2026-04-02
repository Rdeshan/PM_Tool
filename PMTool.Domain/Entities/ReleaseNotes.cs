namespace PMTool.Domain.Entities;

public class ReleaseNotes
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // Markdown or HTML
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedDate { get; set; }
    
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public User? CreatedByUser { get; set; }
}
