using System.ComponentModel.DataAnnotations;

namespace PMTool.Application.DTOs.Backlog;

public class CreateBacklogSubtaskDto
{
    public Guid ParentId { get; set; }

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public int Priority { get; set; } = 3; // Medium

    public Guid? AssigneeId { get; set; }

    public int Status { get; set; } = 1; // To Do
}