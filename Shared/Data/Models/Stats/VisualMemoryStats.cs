using System.Text.Json.Serialization;

namespace PSInzinerija1.Shared.Data.Models.Stats
{
public class VisualMemoryStats : GameStats
    {
        public int GameMistakes { get; set; } = 0;
    }
}