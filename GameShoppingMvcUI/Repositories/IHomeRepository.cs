namespace GameShoppingMvcUI.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Genre>> GetGenres();
        Task<GameListVm> GetGames(string sTerm = "", int genreId = 0, double? maxPrice = null, int page = 1);
    }
}