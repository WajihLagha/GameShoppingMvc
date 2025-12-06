using System.ComponentModel.DataAnnotations.Schema;

namespace GameShoppingMvcUI.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int Quantity { get; set; }

        public Game? Game { get; set; }
    }
}
