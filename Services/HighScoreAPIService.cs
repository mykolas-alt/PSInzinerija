using System.Text;

using PSInzinerija1.Data.Models;
using PSInzinerija1.Enums;

namespace PSInzinerija1.Services
{
    public class HighScoreAPIService(HttpClient httpClient)
    {
        public async Task<int?> GetHighScoreAsync(AvailableGames game)
        {
            string requestUri = $"/api/highscores/{game}";
            var res = await httpClient.GetAsync(requestUri);

            if (res.IsSuccessStatusCode)
            {
                var str = await res.Content.ReadFromJsonAsync<HighScoresEntry>();
                return str?.HighScore;
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

            return res.IsSuccessStatusCode;
        }
    }
}