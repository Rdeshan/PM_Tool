namespace PMTool.Application.DTOs.Board;

public class BoardColumnDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int StatusValue { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateBoardColumnRequest
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UpdateBoardColumnRequest
{
    public Guid ProductId { get; set; }
    public int StatusValue { get; set; }
    public string Name { get; set; } = string.Empty;
}
