namespace PMTool.Application.DTOs.Backlog;

public class WorkTypeDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IconClass { get; set; } = string.Empty;
}

public class WorkTypeOptionDTO
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IconClass { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
}

public class CreateWorkTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IconClass { get; set; } = string.Empty;
}
