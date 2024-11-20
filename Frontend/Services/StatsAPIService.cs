using System.Text;

using PSInzinerija1.Shared.Data.Models;
using Shared.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;
using PSInzinerija1.Shared.Data.Models.Stats;

namespace Frontend.Services
{
    public class StatsAPIService<T> where T : GameStats, new()
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StatsAPIService<T>> _logger;

        public StatsAPIService(IHttpClientFactory httpClientFactory, ILogger<StatsAPIService<T>> logger)
        {
            _httpClient = httpClientFactory?.CreateClient("BackendApi") ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T?> GetStatsAsync(AvailableGames game)
        {
            string url = $"/api/gamestats/{game}/stats";
            if(game == AvailableGames.SimonSays)
            {
                try
                {
                    var stats = await _httpClient.GetFromJsonAsync<SimonSaysStats>(url);
                    return stats as T;
                }
                catch (Exception e)
                {
                    _logger.LogError("Request error: {errorMessage}", e.Message);
                }
            }
            else if(game == AvailableGames.VisualMemory)
            {
                try
                {
                    var stats = await _httpClient.GetFromJsonAsync<VisualMemoryStats>(url);
                    return stats as T;
                }
                catch (Exception e)
                {
                    _logger.LogError("Request error: {errorMessage}", e.Message);
                }
            }
            return null;  
        }

        public async Task SaveStatsAsync(AvailableGames game, T stats)
        {
            string requestUri = $"/api/gamestats/{game}/stats";

            try
            {
                
                var json = JsonSerializer.Serialize(stats);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await _httpClient.PostAsync(requestUri, content);

                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError("Request error: {errorMessage}", result.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Request error: {errorMessage}", e.Message);
            }
        }
    }
}