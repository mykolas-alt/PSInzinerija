using System.Text;
using PSInzinerija1.Enums;


namespace PSInzinerija1.Services
{
    public class GameRulesAPIService(HttpClient httpClient)
    {
        public async Task<string?> GetGameRulesAsync()
        {
            string requestUri = "api/gamerules/stream";
            var res = await httpClient.GetAsync(requestUri);
            var str = "";
            if (res.IsSuccessStatusCode)
            {
                using (var stream = await res.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        str = await reader.ReadToEndAsync();
                    }
                return str;
            }
            else
            {
                str = "Failed to load game rules.";
                return str;
            }
        }
    }
}



