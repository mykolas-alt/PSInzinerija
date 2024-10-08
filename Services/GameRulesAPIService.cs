using System.Text;
using PSInzinerija1.Enums;


namespace PSInzinerija1.Services
{
    public class GameRulesAPIService(HttpClient httpClient)
    {
        public struct rulesReader
        {
            public string rules;
            public HttpResponseMessage res;
            public string requestUri;
        }
        public async Task<string?> GetGameRulesAsync()
        {
            rulesReader ruleGetter = new rulesReader() {rules = "", requestUri = "api/gamerules/stream",};
            ruleGetter.res = await httpClient.GetAsync(ruleGetter.requestUri);
            
            if (ruleGetter.res.IsSuccessStatusCode)
            {
                    using(var stream = await ruleGetter.res.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        ruleGetter.rules = await reader.ReadToEndAsync();
                    }
                return ruleGetter.rules;
            }
            else
            {
                ruleGetter.rules = "Failed to load game rules.";
                return ruleGetter.rules;
            }
        }
    }
}



