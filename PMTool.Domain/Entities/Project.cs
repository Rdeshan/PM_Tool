namespace PMTool.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty; // Unique short code like 'UMS'
    public int Status { get; set; } = 1; // ProjectStatus enum value (Active = 1)
    public string? AvatarUrl { get; set; }
    public string? ColourCode { get; set; } // Hex colour code like '#FF5733'
    public bool IsArchived { get; set; } = false;
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }

    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<UserRole> TeamMembers { get; set; } = new List<UserRole>();
    public ICollection<ProjectBacklog> Backlogs { get; set; } = new List<ProjectBacklog>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
