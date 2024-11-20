using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Shared.Enums;

namespace Backend.Data.Models
{
    [Table("high_scores")]
    [PrimaryKey(nameof(Id), nameof(GameId))]
    public class HighScoresEntry
    {
        [Column("user_id")]
        public required string Id { get; set; }
        [Column("high_score")]
        public required int HighScore { get; set; }
        [Column("game_id")]
        public required AvailableGames GameId { get; set; }
        [Column("record_date")]
        public required DateTime RecordDate { get; set; }
    }
}
