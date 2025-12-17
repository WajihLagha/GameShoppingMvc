namespace GameShoppingMvcUI.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Genre>> GetGenres();
        Task<GameListVm> GetGames(string sTerm = "", int genreId = 0, int page = 1);
    }
}