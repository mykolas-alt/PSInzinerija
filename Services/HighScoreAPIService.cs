using PSInzinerija1.Enums;
using PSInzinerija1.Models;

namespace PSInzinerija1.Services
{
    public class HighScoreAPIService(IHttpClientFactory httpClientFactory) : IHighScoreAPIService
    {
        public async Task<int?> GetHighScoreAsync(AvailableGames gameId, long userId)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("http://localhost:5181");

            string requestUri = $"/api/highscores/{gameId}/{userId}";
            var res = await httpClient.GetAsync(requestUri);

            if (res.IsSuccessStatusCode)
            {
                var str = await res.Content.ReadFromJsonAsync<HighScoresEntry>();
                return str?.HighScore;
            }

            return null;
        }

        public async Task<bool> SaveHighScoreToDbAsync(HighScoresEntry entry)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("http://localhost:5181");

            var content = JsonContent.Create(entry);
            var res = await httpClient.PutAsync($"/api/highscores/{entry.GameId}/{entry.Id}", content);

            return res.IsSuccessStatusCode;
        }
    }
}