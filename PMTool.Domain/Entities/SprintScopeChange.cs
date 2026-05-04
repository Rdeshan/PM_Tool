using System;

namespace PMTool.Domain.Entities;

public class SprintScopeChange
{
    public Guid Id { get; set; }
    public Guid SprintId { get; set; }
    public Guid BacklogItemId { get; set; }
    public string ChangeType { get; set; } = string.Empty; // Added, Removed
    public DateTime ChangeDate { get; set; }
    public Guid ChangedById { get; set; }

    // Navigation properties
    public Sprint? Sprint { get; set; }
    public ProductBacklog? BacklogItem { get; set; }
    public User? ChangedBy { get; set; }
}
