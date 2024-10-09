using System.Text;
using PSInzinerija1.Enums;


namespace PSInzinerija1.Services
{
    public class GameRulesAPIService(HttpClient httpClient)
    {
        
        public async Task<GameInformation> GetGameRulesAsync()
        {
            GameInformation infoGetter = new GameInformation
            {
                rules = "",
                gameName = "Simon Says",
                releaseDate = new DateTime(2024, 9, 27)
            };

            string requestUri = "api/gamerules/stream";
            var res = await httpClient.GetAsync(requestUri);
            
            if (res.IsSuccessStatusCode)
            {
                    using(var stream = await res.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        infoGetter.rules = await reader.ReadToEndAsync();
                    }
                return infoGetter;
            }
            else
            {
                infoGetter.rules = "Failed to load game rules.";
                return infoGetter;
            }
        }
    }
}



