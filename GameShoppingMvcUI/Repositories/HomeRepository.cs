
using Microsoft.EntityFrameworkCore;

namespace GameShoppingMvcUI.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _db.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Game>> GetGames(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();

            var games = await (
                from game in _db.Games
                join genre in _db.Genres on game.GenreId equals genre.Id
                where (string.IsNullOrWhiteSpace(sTerm) ||
                       game.GameName!.ToLower().Contains(sTerm))
                   && (genreId == 0 || game.GenreId == genreId)
                select new Game
                {
                    Id = game.Id,
                    GameName = game.GameName,
                    Price = game.Price,
                    Publisher = game.Publisher,
                    YearOut = game.YearOut,
                    Image = game.Image,
                    GenreId = game.GenreId,
                    GenreName = genre.GenreName
                }
            ).ToListAsync();

            return games;
        }

    }

}
