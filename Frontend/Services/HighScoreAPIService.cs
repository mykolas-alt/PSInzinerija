using System.Text;

using PSInzinerija1.Shared.Data.Models;
using Shared.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Frontend.Services
{
    public class HighScoreAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HighScoreAPIService> _logger;

        public HighScoreAPIService(IHttpClientFactory httpClientFactory, ILogger<HighScoreAPIService> logger)
        {
            _httpClient = httpClientFactory?.CreateClient("BackendApi") ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardEntriesAsync(AvailableGames game)
        {
            string url = $"/api/highscores/{game}/all";

            try
            {
                var leaderboard = await _httpClient.GetFromJsonAsync<List<LeaderboardEntry>>(url);
                return leaderboard ?? [];  // Return empty list if null
            }
            catch (Exception e)
            {
                _logger.LogError("Request error: {errorMessage}", e.Message);
                return [];  // Return empty list in case of error
            }

        }

        public async Task<int?> GetHighScoreAsync(AvailableGames game)
        {
            string requestUri = $"/api/highscores/{game}";
            try
            {
                var result = await _httpClient.GetAsync(requestUri);

                if (result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();

                    JsonNode? node = JsonNode.Parse(json);

                    return node?["highScore"]?.GetValue<int>();
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Request error: {errorMessage}", e.Message);
            }

            return null;
        }

        public async Task<bool> DeleteFromDbAsync(AvailableGames game)
        {
            try
            {
                var res = await _httpClient.DeleteAsync($"/api/highscores/{game}");
                return res.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError("Request error: {errorMessage}", e.Message);
            }

            return false;
        }

        public async Task<bool> SaveHighScoreToDbAsync(AvailableGames game, int newHighScore)
        {
            try
            {
                var content = new StringContent(newHighScore.ToString(), Encoding.UTF8, "application/json");
                var res = await _httpClient.PutAsync($"/api/highscores/{game}", content);

                return res.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError("Request error: {errorMessage}", e.Message);
            }

            return false;
        }
    }
}