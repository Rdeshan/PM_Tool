using PMTool.Application.DTOs.Board;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Board;

public class BoardColumnService : IBoardColumnService
{
    private readonly IBoardColumnRepository _boardColumnRepository;

    public BoardColumnService(IBoardColumnRepository boardColumnRepository)
    {
        _boardColumnRepository = boardColumnRepository;
    }

    public async Task<List<BoardColumnDTO>> GetColumnsByProductAsync(Guid productId)
    {
        var columns = await _boardColumnRepository.GetByProductIdAsync(productId);
        return columns.Select(MapToDto).ToList();
    }

    public async Task<BoardColumnDTO?> CreateColumnAsync(CreateBoardColumnRequest request)
    {
        if (request.ProductId == Guid.Empty)
        {
            return null;
        }

        var name = string.IsNullOrWhiteSpace(request.Name) ? "New container" : request.Name.Trim();
        var existing = await _boardColumnRepository.GetByProductIdAsync(request.ProductId);
        var nextStatus = existing.Count == 0
            ? 5
            : Math.Max(4, existing.Max(x => x.StatusValue)) + 1;

        var column = new BoardColumn
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            StatusValue = nextStatus,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _boardColumnRepository.AddAsync(column);
        return created == null ? null : MapToDto(created);
    }

    public async Task<bool> UpdateColumnAsync(UpdateBoardColumnRequest request)
    {
        if (request.ProductId == Guid.Empty || request.StatusValue <= 4)
        {
            return false;
        }

        var column = await _boardColumnRepository.GetByProductAndStatusAsync(request.ProductId, request.StatusValue);
        if (column == null)
        {
            return false;
        }

        column.Name = string.IsNullOrWhiteSpace(request.Name) ? column.Name : request.Name.Trim();
        column.UpdatedAt = DateTime.UtcNow;

        return await _boardColumnRepository.UpdateAsync(column);
    }

    private static BoardColumnDTO MapToDto(BoardColumn column)
    {
        return new BoardColumnDTO
        {
            Id = column.Id,
            ProductId = column.ProductId,
            StatusValue = column.StatusValue,
            Name = column.Name
        };
    }
}
