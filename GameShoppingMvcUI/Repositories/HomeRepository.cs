
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
        public async Task<GameListVm> GetGames(string sTerm = "", int genreId = 0, int page = 1)
        {
            string sTermOrig = sTerm;
            sTerm = sTerm.ToLower();
            int pageSize = 10;

            var gamesQuery = (
                from game in _db.Games
                join genre in _db.Genres on game.GenreId equals genre.Id
                join stock in _db.Stocks on game.Id equals stock.GameId into stockGroup
                from stock in stockGroup.DefaultIfEmpty()
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
                    GenreName = genre.GenreName,
                    UpdatedDate = game.UpdatedDate,
                    Quantity = stock != null ? stock.Quantity : 0
                }
            );

            var totalGames = await gamesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalGames / pageSize);
            
            // Ensure page is within valid range
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

            var games = await gamesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new GameListVm
            {
                Games = games,
                Genres = await GetGenres(),
                STerm = sTermOrig,
                GenreId = genreId,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };
        }

    }

}
