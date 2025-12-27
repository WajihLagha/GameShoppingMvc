using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShoppingMvcUI.Models
{
    [Table("PackGame")]
    public class PackGame
    {
        public int Id { get; set; }
        
        [Required]
        public int PackId { get; set; }
        
        [Required]
        public int GameId { get; set; }
        
        // Navigation properties
        public Pack? Pack { get; set; }
        public Game? Game { get; set; }
    }
}
