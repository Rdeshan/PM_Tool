using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IBoardColumnRepository
{
    Task<List<BoardColumn>> GetByProductIdAsync(Guid productId);
    Task<BoardColumn?> GetByProductAndStatusAsync(Guid productId, int statusValue);
    Task<BoardColumn?> AddAsync(BoardColumn column);
    Task<bool> UpdateAsync(BoardColumn column);
    Task<bool> DeleteAsync(Guid productId, int statusValue);
}
