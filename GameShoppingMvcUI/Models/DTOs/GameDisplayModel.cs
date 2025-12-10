namespace GameShoppingMvcUI.Models.DTOs
{
    public class GameDisplayModel
    {
        public IEnumerable<Game>? Games { get; set; }
        public IEnumerable<Genre>? Genres { get; set; }
    }
}
