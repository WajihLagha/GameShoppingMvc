using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameShoppingMvcUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPackController : Controller
    {
        private readonly IPackRepository _packRepo;
        private readonly IHomeRepository _homeRepo;

        public AdminPackController(IPackRepository packRepo, IHomeRepository homeRepo)
        {
            _packRepo = packRepo;
            _homeRepo = homeRepo;
        }

        // GET: AdminPack
        public async Task<IActionResult> Index()
        {
            var packs = await _packRepo.GetAllPacks();
            return View(packs);
        }

        // GET: AdminPack/Create
        public async Task<IActionResult> Create()
        {
            var games = await _homeRepo.GetGames();
            ViewBag.Games = games;
            return View();
        }

        // POST: AdminPack/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pack pack, List<int> selectedGames)
        {
            if (ModelState.IsValid)
            {
                if (selectedGames == null || selectedGames.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please select at least one game for the pack.";
                    var games = await _homeRepo.GetGames();
                    ViewBag.Games = games;
                    return View(pack);
                }

                var success = await _packRepo.AddPack(pack, selectedGames);
                if (success)
                {
                    TempData["SuccessMessage"] = "Pack created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["ErrorMessage"] = "Failed to create pack. Please try again.";
            }

            var allGames = await _homeRepo.GetGames();
            ViewBag.Games = allGames;
            return View(pack);
        }

        // GET: AdminPack/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var pack = await _packRepo.GetPackById(id);
            if (pack == null)
            {
                TempData["ErrorMessage"] = "Pack not found.";
                return RedirectToAction(nameof(Index));
            }

            var games = await _homeRepo.GetGames();
            ViewBag.Games = games;
            
            // Get selected game IDs
            ViewBag.SelectedGameIds = pack.PackGames?.Select(pg => pg.GameId).ToList() ?? new List<int>();
            
            return View(pack);
        }

        // POST: AdminPack/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pack pack, List<int> selectedGames)
        {
            if (id != pack.Id)
            {
                TempData["ErrorMessage"] = "Invalid pack ID.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                if (selectedGames == null || selectedGames.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please select at least one game for the pack.";
                    var games = await _homeRepo.GetGames();
                    ViewBag.Games = games;
                    var existingPack = await _packRepo.GetPackById(id);
                    ViewBag.SelectedGameIds = existingPack?.PackGames?.Select(pg => pg.GameId).ToList() ?? new List<int>();
                    return View(pack);
                }

                var success = await _packRepo.UpdatePack(pack, selectedGames);
                if (success)
                {
                    TempData["SuccessMessage"] = "Pack updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["ErrorMessage"] = "Failed to update pack. Please try again.";
            }

            var allGames = await _homeRepo.GetGames();
            ViewBag.Games = allGames;
            var currentPack = await _packRepo.GetPackById(id);
            ViewBag.SelectedGameIds = currentPack?.PackGames?.Select(pg => pg.GameId).ToList() ?? new List<int>();
            return View(pack);
        }

        // GET: AdminPack/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var pack = await _packRepo.GetPackById(id);
            if (pack == null)
            {
                TempData["ErrorMessage"] = "Pack not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(pack);
        }

        // POST: AdminPack/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _packRepo.DeletePack(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Pack deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete pack. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
