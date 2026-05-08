using System.ComponentModel.DataAnnotations;

namespace PMTool.Application.DTOs.Comments
{
    // NEW: Create comment DTO
    public class CreateCommentDto
    {
        // NEW: Comment text
        [Required]
        public string Comment { get; set; } = string.Empty;

        // NEW: Parent work item
        public int WorkItemId { get; set; }

        // NEW: User
        public Guid UserId { get; set; }
    }
}