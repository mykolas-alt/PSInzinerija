
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Shared.Enums;

namespace Backend.Data.Models
{
    [Table("game_stats")]
    [PrimaryKey(nameof(Id), nameof(GameId))]
    public class GameStatisticsEntry
    {
        [Column("user_id")]
        public required string Id { get; set; } = default!;
        [Column("game_id")]
        public required AvailableGames GameId { get; set; }
        [Column("recent_score")]
        public int RecentScore { get; set; }
        [Column("mistakes")]
        public int? Mistakes { get; set; }
        [Column("recent_time")]
        public TimeSpan? TimePlayed { get; set; }
    }
}