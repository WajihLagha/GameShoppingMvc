
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
        
        public async Task<GameListVm> GetGames(string sTerm = "", int genreId = 0, double? minPrice = null, double? maxPrice = null, int? minYear = null, int? maxYear = null, string sortBy = "name", int page = 1)
        {
            string sTermOrig = sTerm;
            sTerm = sTerm.ToLower();
            int pageSize = 12;

            // Get min and max values from database for filter ranges
            var hasGames = await _db.Games.AnyAsync();
            var maxPriceInDb = hasGames ? await _db.Games.MaxAsync(g => g.Price) : 100;
            var minPriceInDb = hasGames ? await _db.Games.MinAsync(g => g.Price) : 0;
            var maxYearInDb = hasGames ? await _db.Games.MaxAsync(g => g.YearOut) : DateTime.Now.Year;
            var minYearInDb = hasGames ? await _db.Games.MinAsync(g => g.YearOut) : 2000;

            var gamesQuery = (
                from game in _db.Games
                join genre in _db.Genres on game.GenreId equals genre.Id
                join stock in _db.Stocks on game.Id equals stock.GameId into stockGroup
                from stock in stockGroup.DefaultIfEmpty()
                where (string.IsNullOrWhiteSpace(sTerm) ||
                       game.GameName!.ToLower().Contains(sTerm))
                   && (genreId == 0 || game.GenreId == genreId)
                   && (minPrice == null || game.Price >= minPrice)
                   && (maxPrice == null || game.Price <= maxPrice)
                   && (minYear == null || game.YearOut >= minYear)
                   && (maxYear == null || game.YearOut <= maxYear)
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
                    Quantity = stock != null ? stock.Quantity : 0,
                    DiscountPercent = game.DiscountPercent // Ensure this is mapped
                }
            );

            // Apply sorting
            gamesQuery = sortBy?.ToLower() switch
            {
                "price-asc" => gamesQuery.OrderBy(g => g.Price),
                "price-desc" => gamesQuery.OrderByDescending(g => g.Price),
                "year-asc" => gamesQuery.OrderBy(g => g.YearOut),
                "year-desc" => gamesQuery.OrderByDescending(g => g.YearOut),
                "updated" => gamesQuery.OrderByDescending(g => g.UpdatedDate),
                _ => gamesQuery.OrderBy(g => g.GameName) // default: name ascending
            };

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
                PageSize = pageSize,
                MaxPrice = maxPriceInDb,
                MinPrice = minPriceInDb,
                SelectedMaxPrice = maxPrice,
                SelectedMinPrice = minPrice,
                  MaxYear = maxYearInDb,
                MinYear = minYearInDb,
                SelectedMaxYear = maxYear,
                SelectedMinYear = minYear,
                SortBy = sortBy
            };
        }

    }

}
