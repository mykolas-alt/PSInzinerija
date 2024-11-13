using System.Text;

using PSInzinerija1.Shared.Data.Models;
using PSInzinerija1.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Frontend.Services
{
    public class HighScoreAPIService(HttpClient httpClient)
    {
        public async Task<List<LeaderboardEntry>> GetLeaderboardEntriesAsync(AvailableGames game)
        {
            string url = $"/api/highscores/{game}/all";

            try
            {
                var leaderboard = await httpClient.GetFromJsonAsync<List<LeaderboardEntry>>(url);
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
            var res = await httpClient.GetAsync(requestUri);

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
            var res = await httpClient.DeleteAsync($"/api/highscores/{game}");

            return res.IsSuccessStatusCode;
        }

        public async Task<bool> SaveHighScoreToDbAsync(AvailableGames game, int newHighScore)
        {
            var content = new StringContent(newHighScore.ToString(), Encoding.UTF8, "application/json");
            var res = await httpClient.PutAsync($"/api/highscores/{game}", content);

            Console.WriteLine(httpClient.DefaultRequestHeaders);

            return res.IsSuccessStatusCode;
        }
    }
}