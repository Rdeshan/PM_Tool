using PMTool.Application.DTOs.Comments;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Application.Services.Subtask
{
    // NEW: Comment service
    public class CommentService : ICommentService
    {
        // NEW: Database context
        private readonly AppDbContext _context;

        // NEW: Constructor
        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        // NEW: Add comment
        public async Task AddCommentAsync(CreateCommentDto dto)
        {
            var comment = new WorkComment
            {
                Comment = dto.Comment,
                WorkItemId = dto.WorkItemId,
                UserId = dto.UserId
            };

            _context.WorkComments.Add(comment);

            await _context.SaveChangesAsync();
        }
        public async Task DeleteCommentAsync(Guid commentId)
        {
            var comment = await _context.WorkComments.FindAsync(commentId);
            if (comment != null)
            {
                _context.WorkComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}