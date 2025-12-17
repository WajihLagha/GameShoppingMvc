namespace GameShoppingMvcUI.Models.DTOs
{
    public class GameListVm
    {
        public IEnumerable<Game> Games { get; set; } = new List<Game>();
        public IEnumerable<Genre> Genres { get; set; } = new List<Genre>();
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
