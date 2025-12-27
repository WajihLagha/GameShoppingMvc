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
        public int PageSize { get; set; } = 12;
        
        // Price filters
        public double? SelectedMinPrice { get; set; }
        public double? SelectedMaxPrice { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        
        // Year filters
        public int? SelectedMinYear { get; set; }
        public int? SelectedMaxYear { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        
        // Sorting
        public string SortBy { get; set; } = "name";
    }
}
