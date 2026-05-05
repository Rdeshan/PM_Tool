using System;
using System.Collections.Generic;

namespace PMTool.Application.DTOs.Sprint;

public class SprintDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Goal { get; set; } = string.Empty;
    public int Status { get; set; }
    public int TotalPoints { get; set; }
    public int ItemCount { get; set; }
}

public class CreateSprintRequest
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Goal { get; set; } = string.Empty;
}
