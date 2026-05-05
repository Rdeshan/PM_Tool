using System;
using System.Collections.Generic;

namespace PMTool.Domain.Entities;

public class Sprint
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Goal { get; set; } = string.Empty;
    public int Status { get; set; } // 1=Draft/Planned, 2=Active, 3=Completed

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public ICollection<ProductBacklog> BacklogItems { get; set; } = new List<ProductBacklog>();
    public ICollection<SprintScopeChange> ScopeChanges { get; set; } = new List<SprintScopeChange>();
}
