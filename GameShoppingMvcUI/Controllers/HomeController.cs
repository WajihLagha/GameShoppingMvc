using System.Diagnostics;
using GameShoppingMvcUI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

using GameShoppingMvcUI.Services;

namespace GameShoppingMvcUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        private readonly IGeminiService _geminiService;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository, IGeminiService geminiService)
        {
            _logger = logger;
            _homeRepository = homeRepository;
            _geminiService = geminiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecommendation(string prompt, string genre)
        {
             if (string.IsNullOrWhiteSpace(prompt))
             {
                 return BadRequest("Please provide a description of the game you want.");
             }

             string fullPrompt = $"Recommend 3 games based on this description: '{prompt}'";
             if (!string.IsNullOrWhiteSpace(genre) && genre != "0")
             {
                 // Assuming genre is passed as ID, we might need name, but for now let's append it contextually
                 // or just use the prompt. If genre is a name from the UI, great.
                 // The UI will likely pass the genre Name or ID. Let's assume Name or ID. 
                 // If ID, we'd need to look it up, but let's keep it simple and ask UI to pass name or just rely on prompt.
                 // Actually, let's append it if provided.
                 fullPrompt += $" and in the genre/style of '{genre}'";
             }
             fullPrompt += ". Provide the response in a concise format with Game Name and a short reason why.";

             var recommendation = await _geminiService.GetRecommendationAsync(fullPrompt);
             return Ok(recommendation);
        }

        public async Task<IActionResult> Index(string sTerm="",int genreId = 0, double? maxPrice = null, int page = 1)
        {
            GameListVm model = await _homeRepository.GetGames(sTerm, genreId, maxPrice, page);
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
