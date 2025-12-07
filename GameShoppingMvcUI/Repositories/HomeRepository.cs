
using Microsoft.EntityFrameworkCore;

namespace GameShoppingMvcUI.Repositories
{
    public class HomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Game>> DisplayGames(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Game> games = await (from game in _db.Games
                         join genre in _db.Genres
                         on game.GenreId equals genre.Id
                         where (string.IsNullOrEmpty(sTerm) || (game!=null && game.GameName!.ToLower().Contains(sTerm)))
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
            if (genreId > 0)
            {
                games = games.Where(g => g.GenreId == genreId).ToList();
            }
            return games;
        }
    }

}
