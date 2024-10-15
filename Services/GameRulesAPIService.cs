using System.Text;
using PSInzinerija1.Enums;
using PSInzinerija1.Games.SimonSays;

namespace PSInzinerija1.Services
{
    public class GameRulesAPIService(HttpClient httpClient)
    {
        
        public async Task<GameInfoStruct> GetGameRulesAsync()
        {
            GameInfoStruct gameInfo = new GameInfoStruct
            {
                rules = "",
                gameName = "Simon Says",
                releaseDate = new DateTime(2024, 9, 27)
            };

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/GameRules/SimonSaysRules.txt");

            if (!System.IO.File.Exists(filePath))
            {
                return gameInfo; //grazina tuscias taisykles
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                gameInfo.rules = await reader.ReadToEndAsync();
            }

            return gameInfo; //grazina perskaicius
        }
    }
}



