using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShoppingMvcUI.Models
{
    [Table("Genre")]
    public class Genre
    {
        public int id { get; set; }

        [Required]
        [MaxLength(45)]
        public string? GenreName { get; set; }
        public List<Game>? Games { get; set; }
    }
}
