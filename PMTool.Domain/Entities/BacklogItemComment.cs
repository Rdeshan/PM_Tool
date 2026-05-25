using System.ComponentModel.DataAnnotations;

namespace PMTool.Domain.Entities;

public class BacklogItemComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BacklogItemId { get; set; }
    public Guid AuthorId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Body { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ProductBacklog? BacklogItem { get; set; }
    public User? Author { get; set; }
}
