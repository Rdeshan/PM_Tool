using System.ComponentModel.DataAnnotations;

namespace PMTool.Domain.Entities
{
    // Models/WorkType.cs
    public class WorkType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsDefault { get; set; } = false;
    }
}
