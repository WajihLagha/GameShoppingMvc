using Microsoft.EntityFrameworkCore;

namespace GameShoppingMvcUI.Repositories
{
    public class PackRepository : IPackRepository
    {
        private readonly ApplicationDbContext _db;

        public PackRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Pack>> GetAllPacks()
        {
            try
            {
                return await _db.Packs
                    .Include(p => p.PackGames!)
                    .ThenInclude(pg => pg.Game)
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all packs: {ex.Message}");
                return new List<Pack>();
            }
        }

        public async Task<Pack?> GetPackById(int id)
        {
            try
            {
                return await _db.Packs
                    .Include(p => p.PackGames!)
                    .ThenInclude(pg => pg.Game)
                    .ThenInclude(g => g!.Genre)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pack by id: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddPack(Pack pack, List<int> gameIds)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Packs.Add(pack);
                await _db.SaveChangesAsync();

                // Add pack games
                foreach (var gameId in gameIds)
                {
                    var packGame = new PackGame
                    {
                        PackId = pack.Id,
                        GameId = gameId
                    };
                    _db.PackGames.Add(packGame);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error adding pack: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdatePack(Pack pack, List<int> gameIds)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var existingPack = await _db.Packs
                    .Include(p => p.PackGames)
                    .FirstOrDefaultAsync(p => p.Id == pack.Id);

                if (existingPack == null)
                    return false;

                // Update pack details
                existingPack.PackName = pack.PackName;
                existingPack.Description = pack.Description;
                existingPack.Price = pack.Price;
                existingPack.Image = pack.Image;
                existingPack.IsActive = pack.IsActive;

                // Remove existing pack games
                if (existingPack.PackGames != null)
                {
                    _db.PackGames.RemoveRange(existingPack.PackGames);
                }

                // Add new pack games
                foreach (var gameId in gameIds)
                {
                    var packGame = new PackGame
                    {
                        PackId = pack.Id,
                        GameId = gameId
                    };
                    _db.PackGames.Add(packGame);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error updating pack: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePack(int id)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var pack = await _db.Packs
                    .Include(p => p.PackGames)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pack == null)
                    return false;

                // Remove pack games first
                if (pack.PackGames != null)
                {
                    _db.PackGames.RemoveRange(pack.PackGames);
                }

                // Remove pack
                _db.Packs.Remove(pack);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error deleting pack: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Pack>> GetActivePacksForGames(List<int> gameIds)
        {
            try
            {
                // Get packs that contain any of the specified games
                return await _db.Packs
                    .Where(p => p.IsActive && p.PackGames!.Any(pg => gameIds.Contains(pg.GameId)))
                    .Include(p => p.PackGames!)
                    .ThenInclude(pg => pg.Game)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting active packs for games: {ex.Message}");
                return new List<Pack>();
            }
        }
    }
}
