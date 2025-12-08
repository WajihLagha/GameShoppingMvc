namespace GameShoppingMvcUI.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Game>> GetGames(string sTerm = "", int genreId = 0);
    }
}