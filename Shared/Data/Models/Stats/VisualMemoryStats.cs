using System.Text.Json.Serialization;

namespace PSInzinerija1.Shared.Data.Models.Stats
{
public class VisualMemoryStats : Stats
    {
        public int GameMistakes { get; set; } = 0;
    }
}