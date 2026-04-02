namespace PMTool.Domain.Entities;

public class TeamMember
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }

    // Navigation properties
    public Team? Team { get; set; }
    public User? User { get; set; }
}
