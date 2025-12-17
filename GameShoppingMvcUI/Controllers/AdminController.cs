using GameShoppingMvcUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameShoppingMvcUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var games = await _db.Games
                .Include(g => g.Genre)
                .Select(g => new Game
                {
                    Id = g.Id,
                    GameName = g.GameName,
                    Price = g.Price,
                    Publisher = g.Publisher,
                    YearOut = g.YearOut,
                    Image = g.Image,
                    GenreId = g.GenreId,
                    GenreName = g.Genre!.GenreName,
                    UpdatedDate = g.UpdatedDate,
                    Quantity = _db.Stocks.Where(s => s.GameId == g.Id).Select(s => s.Quantity).FirstOrDefault()
                })
                .ToListAsync();

            return View(games);
        }

        // GET: Admin/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "Id", "GenreName");
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Game game, int initialStock = 0)
        {
            if (ModelState.IsValid)
            {
                game.UpdatedDate = DateTime.Now;
                _db.Games.Add(game);
                await _db.SaveChangesAsync();

                // Add initial stock
                var stock = new Stock
                {
                    GameId = game.Id,
                    Quantity = initialStock
                };
                _db.Stocks.Add(stock);
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Game '{game.GameName}' created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "Id", "GenreName", game.GenreId);
            ViewBag.InitialStock = initialStock;
            return View(game);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _db.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.GameId == id);
            game.Quantity = stock?.Quantity ?? 0;

            ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "Id", "GenreName", game.GenreId);
            return View(game);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Game game, int? quantity)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    game.UpdatedDate = DateTime.Now;
                    _db.Update(game);
                    await _db.SaveChangesAsync();

                    // Update stock
                    if (quantity.HasValue)
                    {
                        var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.GameId == id);
                        if (stock != null)
                        {
                            stock.Quantity = quantity.Value;
                            _db.Update(stock);
                        }
                        else
                        {
                            stock = new Stock { GameId = id, Quantity = quantity.Value };
                            _db.Stocks.Add(stock);
                        }
                        await _db.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = $"Game '{game.GameName}' updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "Id", "GenreName", game.GenreId);
            game.Quantity = quantity ?? 0;
            return View(game);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _db.Games
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.GameId == id);
            game.Quantity = stock?.Quantity ?? 0;

            return View(game);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _db.Games.FindAsync(id);
            if (game != null)
            {
                // Remove associated stock
                var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.GameId == id);
                if (stock != null)
                {
                    _db.Stocks.Remove(stock);
                }

                _db.Games.Remove(game);
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Game '{game.GameName}' deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _db.Games.Any(e => e.Id == id);
        }
    }
}
