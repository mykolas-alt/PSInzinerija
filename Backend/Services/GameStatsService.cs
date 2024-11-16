using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PSInzinerija1.Data.ApplicationDbContext;
using PSInzinerija1.Data.Models;
using PSInzinerija1.Shared.Data.Models.Stats;
using PSInzinerija1.Enums;

namespace PSInzinerija1.Services
{
    public class GameStatsService<T> where T : Stats, new()
    {
        private readonly ApplicationDbContext _context;

        public GameStatsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetStatsAsync(string userId, AvailableGames gameId)
        {
            var highScoresEntry = await _context.HighScores
                .Where(e => e.Id == userId && e.GameId == gameId)
                .OrderByDescending(e => e.RecordDate)
                .FirstOrDefaultAsync();

            if (highScoresEntry == null)
            {
                return new T();
            }

            var stats = new T
            {
                HighScore = highScoresEntry.HighScore,
                RecentScores = highScoresEntry.RecentScores
            };

            if (stats is VisualMemoryStats visualMemoryStats)
            {
                visualMemoryStats.GameMistakes = highScoresEntry.Mistakes;
            }
            else if (stats is SimonSaysStats simonSaysStats)
            {
                simonSaysStats.FastestTimes = highScoresEntry.FastestTimes;
            }

            return stats;
        }
    }
}