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
        public double Price { get; set; }
        [Required]
        [MaxLength(45)]
        public string? Publisher { get; set; }
        public int YearOut { get; set; }
        public string? Image { get; set; }
        [Required]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public List<CartDetail>? CartDetails { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }

    }
}
