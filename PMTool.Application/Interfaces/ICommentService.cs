using PMTool.Application.DTOs.Comments;

namespace PMTool.Application.Interfaces
{
    // NEW: Comment service contract
    public interface ICommentService
    {
        // NEW: Add comment
        Task AddCommentAsync(CreateCommentDto dto);
        Task DeleteCommentAsync(Guid commentId);
    }
}