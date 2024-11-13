using System.Text.Json.Serialization;

namespace PSInzinerija1.Data.Models
{
    public class GameStats<T> where T : class, new()
    {
        public int HighScore { get; set; }
        public int[] RecentScores { get; set; } = new int[3];
        
        public T SpecificStats { get; set; }

        public GameStats(T specificStats)
        {
            SpecificStats = specificStats;
        }
    }

    public class SimonSaysStats
    {
        public int[] FastestTimes { get; set; } = new int[3];
    }
    

    public class VisualMemoryStats
    {
        public int[] GameMistakes { get; set; } = new int[3];
    }
}