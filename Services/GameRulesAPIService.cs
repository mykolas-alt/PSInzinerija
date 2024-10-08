using System.Text;
using PSInzinerija1.Enums;


namespace PSInzinerija1.Services
{
    public class GameRulesAPIService(HttpClient httpClient)
    {
        
        public async Task<rulesReader> GetGameRulesAsync()
        {
            rulesReader ruleGetter = new rulesReader() {rules = "", gameName = "", releaseDate = new DateTime()};
            ruleGetter.gameName = "Simon Says"; 
            ruleGetter.releaseDate = new DateTime(2024, 9, 27);

            string requestUri = "api/gamerules/stream";
            var res = await httpClient.GetAsync(requestUri);
            
            if (res.IsSuccessStatusCode)
            {
                    using(var stream = await res.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        ruleGetter.rules = await reader.ReadToEndAsync();
                    }
                return ruleGetter;
            }
            else
            {
                ruleGetter.rules = "Failed to load game rules.";
                return ruleGetter;
            }
        }
    }
}



