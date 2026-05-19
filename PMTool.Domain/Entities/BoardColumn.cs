namespace PMTool.Domain.Entities;

public class BoardColumn
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int StatusValue { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Product? Product { get; set; }
}
