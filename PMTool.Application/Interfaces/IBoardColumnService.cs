using PMTool.Application.DTOs.Board;

namespace PMTool.Application.Interfaces;

public interface IBoardColumnService
{
    Task<List<BoardColumnDTO>> GetColumnsByProductAsync(Guid productId);
    Task<BoardColumnDTO?> CreateColumnAsync(CreateBoardColumnRequest request);
    Task<bool> UpdateColumnAsync(UpdateBoardColumnRequest request);
    Task<bool> DeleteColumnAsync(Guid productId, int statusValue);
}
