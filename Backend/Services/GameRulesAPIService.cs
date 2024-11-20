using System.Text;

using PSInzinerija1.Enums;
using PSInzinerija1.Shared.Data.Models;

namespace PSInzinerija1.Services
{
    public class GameRulesAPIService(HttpClient httpClient)
    {

        public async Task<GameInfo> GetGameRulesAsync()
        {
            GameInfo gameInfo = new GameInfo
            {
                Rules = "",
                GameName = "Simon Says",
                ReleaseDate = new DateTime(2024, 9, 27)
            };

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "GameRules/SimonSaysRules.txt");

            if (!System.IO.File.Exists(filePath))
            {
                return gameInfo; //grazina tuscias taisykles
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                gameInfo.Rules = await reader.ReadToEndAsync();
            }

            return gameInfo; //grazina perskaicius
        }
    }
}



