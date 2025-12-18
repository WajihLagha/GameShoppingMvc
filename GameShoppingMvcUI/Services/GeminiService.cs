using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameShoppingMvcUI.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetRecommendationAsync(string prompt)
        {
            var apiKey = _configuration["GeminiApiKey"];
            // Fallback if no key configured, though we removed partial check from appsettings previously,
            // the secrets might be missing or key might be invalid.
            if (string.IsNullOrEmpty(apiKey))
            {
                return GetStaticRecommendation();
            }

            var requestUrl = $"{BaseUrl}?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await _httpClient.PostAsync(requestUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    // Fallback on error (like 429 Quota Exceeded)
                    return GetStaticRecommendation();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);

                if (geminiResponse?.Candidates != null && geminiResponse.Candidates.Length > 0)
                {
                    return geminiResponse.Candidates[0].Content?.Parts?[0]?.Text ?? GetStaticRecommendation();
                }

                return GetStaticRecommendation();
            }
            catch (Exception)
            {
                // Fallback on exception
                return GetStaticRecommendation();
            }
        }

        private string GetStaticRecommendation()
        {
            return "**(Curated Picks) High demand on AI service, here are our top recommendations:**\n\n" +
                   "1. **Elden Ring**\n   - A masterpiece open-world RPG with challenging combat and deep lore.\n" +
                   "2. **Stardew Valley**\n   - The ultimate relaxing farming simulator, perfect for unwinding.\n" +
                   "3. **Hades**\n   - A stylish rogue-like with fast-paced action and engaging storytelling.\n\n" +
                   "*Note: These are static recommendations as our AI brain is taking a quick nap.*";
        }

        // Helper classes for JSON deserialization
        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public Candidate[] Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content Content { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public Part[] Parts { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }
    }
}
