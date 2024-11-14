using System.Text;

using PSInzinerija1.Shared.Data.Models;
using PSInzinerija1.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Frontend.Services
{
    public class HighScoreAPIService(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BackendApi");

        public async Task<List<LeaderboardEntry>> GetLeaderboardEntriesAsync(AvailableGames game)
        {
            string url = $"/api/highscores/{game}/all";

            try
            {
                var leaderboard = await _httpClient.GetFromJsonAsync<List<LeaderboardEntry>>(url);
                return leaderboard ?? [];  // Return empty list if null
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return [];  // Return empty list in case of error
            }

        }

        public async Task<int?> GetHighScoreAsync(AvailableGames game)
        {
            string requestUri = $"/api/highscores/{game}";
            var res = await _httpClient.GetAsync(requestUri);

            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();

                JsonNode? node = JsonNode.Parse(json);

                return node?["highScore"]?.GetValue<int>();
            }

            return null;
        }

        public async Task<bool> DeleteFromDbAsync(AvailableGames game)
        {
            var res = await _httpClient.DeleteAsync($"/api/highscores/{game}");

            return res.IsSuccessStatusCode;
        }

        public async Task<bool> SaveHighScoreToDbAsync(AvailableGames game, int newHighScore)
        {
            var content = new StringContent(newHighScore.ToString(), Encoding.UTF8, "application/json");
            var res = await _httpClient.PutAsync($"/api/highscores/{game}", content);

            Console.WriteLine(_httpClient.DefaultRequestHeaders);

            return res.IsSuccessStatusCode;
        }
    }
}