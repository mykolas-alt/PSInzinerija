using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using PSInzinerija1.Enums;

namespace Backend.Data.Models
{
    [Table("high_scores")]
    [PrimaryKey(nameof(Id), nameof(GameId))]
    public class HighScoresEntry
    {
        [Column("user_id")]
        public required string Id { get; set; } = default!;
        [Column("high_score")]
        public required int HighScore { get; set; }
        [Column("game_id")]
        public required AvailableGames GameId { get; set; }
        [Column("record_date")]
        public required DateTime RecordDate { get; set; }
        public int Mistakes { get; set; }
        [Column("recent_scores")]
        public int RecentScores { get; set; }
        [Column("game_id")]
        public AvailableGames GameId { get; set; }
        [Column("fastest_times")]
        public int MostRecentTime { get; set; } 
        [Column("record_date")]
    }
}
