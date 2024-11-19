using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;
using Shared.Data.Models.Stats;
using Shared.Enums;

namespace PSInzinerija1.Services
{
    public class GameStatsService<T> where T : Stats, new()
    {
        private readonly ApplicationDbContext _context;

        public GameStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fetch Stats for a Specific User and Game
        public async Task<T> GetStatsAsync(string userId, AvailableGames gameId)
        {
            var highScoresEntry = await _context.HighScores
                .Where(e => e.Id == userId && e.GameId == gameId)
                .OrderByDescending(e => e.RecordDate) // Ensure the most recent entry
                .FirstOrDefaultAsync();

            if (highScoresEntry == null)
            {
                return new T(); // Return a default instance if no data exists
            }

            var stats = new T
            {
                HighScore = highScoresEntry.HighScore,
                RecentScore = highScoresEntry.RecentScore
            };

            // Populate game-specific fields
            if (stats is VisualMemoryStats visualMemoryStats)
            {
                visualMemoryStats.GameMistakes = highScoresEntry.Mistakes;
            }
            else if (stats is SimonSaysStats simonSaysStats)
            {
                simonSaysStats.TimePlayed = highScoresEntry.TimePlayed;
            }

            return stats;
        }

        // Save Stats for a Specific User and Game
        public async Task SaveStatsAsync(string userId, AvailableGames gameId, T stats)
        {
            var highScoresEntry = await _context.HighScores
                .FirstOrDefaultAsync(e => e.Id == userId && e.GameId == gameId);

            if (highScoresEntry == null)
            {
                // Create a new database entry if one doesn't exist
                highScoresEntry = new HighScoresEntry
                {
                    Id = userId,
                    GameId = gameId,
                    RecordDate = DateTime.UtcNow
                };
                _context.HighScores.Add(highScoresEntry);
            }

            // Update common stats
            highScoresEntry.HighScore = stats.HighScore;
            highScoresEntry.RecentScores = stats.RecentScores;

            // Update game-specific stats
            if (stats is VisualMemoryStats visualMemoryStats)
            {
                highScoresEntry.Mistakes = visualMemoryStats.GameMistakes;
            }
            else if (stats is SimonSaysStats simonSaysStats)
            {
                highScoresEntry.TimePlayed = simonSaysStats.TimePlayed;
            }

            await _context.SaveChangesAsync();
        }
    }
}
