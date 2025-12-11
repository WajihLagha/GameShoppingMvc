using System.Diagnostics;
using GameShoppingMvcUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GameShoppingMvcUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sTerm="",int genreId = 0)
        {
            IEnumerable<Game> games = await _homeRepository.GetGames(sTerm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.GetGenres();
            GameDisplayModel model = new GameDisplayModel
            {
                Games = games,
                Genres = genres,
                STearm = sTerm,
                GenreId = genreId
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
