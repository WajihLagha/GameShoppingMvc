using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShoppingMvcUI.Models
{
    [Table("Pack")]
    public class Pack
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string? PackName { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public double Price { get; set; }
        
        public string? Image { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<PackGame>? PackGames { get; set; }
    }
}
