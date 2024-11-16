using System.Text.Json.Serialization;

namespace PSInzinerija1.Shared.Data.Models.Stats
{
    public class Stats
    {
        public int HighScore { get; set; }
        public int[] RecentScores { get; set; } = new int[3];
    }
}