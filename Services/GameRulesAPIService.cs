using PSInzinerija1.Enums;


namespace PSInzinerija1.Services
{
    public class GameRulesAPIService : IGameRulesAPIService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public GameRulesAPIService(IHttpClientFactory httpClientFactory) 
        {
            this.httpClientFactory = httpClientFactory;
        }

        private string? rules;

        public async Task<string?> GetGameRulesAsync()
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("http://localhost:5181");
                var response = await httpClient.GetAsync("api/gamerules/stream");

                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        rules = await reader.ReadToEndAsync();
                    }
                    return rules;
                }
                else
                {
                    rules = "Failed to load game rules.";
                    return rules;
                }
            }
            catch (Exception ex)
            {
                rules = $"Error: {ex.Message}";
                return rules;
            }

            
        }
    }
}
