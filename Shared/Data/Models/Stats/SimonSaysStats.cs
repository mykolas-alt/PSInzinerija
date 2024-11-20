using System.Text.Json.Serialization;

namespace PSInzinerija1.Shared.Data.Models.Stats
{
public class SimonSaysStats : GameStats
    {
        public TimeSpan TimePlayed { get; set; }
    }
}