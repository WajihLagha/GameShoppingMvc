namespace GameShoppingMvcUI.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Genre>> GetGenres();
        Task<GameListVm> GetGames(string sTerm = "", int genreId = 0, double? minPrice = null, double? maxPrice = null, int? minYear = null, int? maxYear = null, string sortBy = "name", int page = 1);
    }
}