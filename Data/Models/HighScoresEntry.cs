using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using PSInzinerija1.Enums;

namespace PSInzinerija1.Data.Models
{
    [Table("high_scores")]
    [PrimaryKey(nameof(Id), nameof(GameId))]
    public class HighScoresEntry
    {
        [Column("user_id")]
        public string Id { get; set; } = default!;
        [Column("high_score")]
        public int HighScore { get; set; }
        [Column("game_id")]
        public AvailableGames GameId { get; set; }
        [Column("record_date")]
        public DateTime RecordDate { get; set; }
    }
}
