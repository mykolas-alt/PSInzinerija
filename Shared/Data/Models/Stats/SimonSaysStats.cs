using System.Text.Json.Serialization;

namespace PSInzinerija1.Shared.Data.Models.Stats
{
public class SimonSaysStats : Stats
    {
        public int[] FastestTimes { get; set; } = new int[3];
    }
}