using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShoppingMvcUI.Models
{
    [Table("Game")]
    public class Game
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(45)]
        public string? GameName { get; set; }
        [Required]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

    }
}
