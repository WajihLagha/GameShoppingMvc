namespace GameShoppingMvcUI.Services
{
    public interface IGeminiService
    {
        Task<string> GetRecommendationAsync(string prompt);
    }
}
