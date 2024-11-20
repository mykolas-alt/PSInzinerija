using System.Security.Claims;

using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Shared.Enums;
using PSInzinerija1.Shared.Data.Models.Stats;


namespace Backend.Services
{
    public class GameStatsService<T> where T : GameStats, new()
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<GameStatsService<T>> _logger;

        public GameStatsService(ApplicationDbContext context, UserManager<User> userManager, ILogger<GameStatsService<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Fetch Stats for a Specific User and Game
        public async Task<T?> GetUserStatsAsync(AvailableGames gameId, ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);

            if (userId == null)
            {
                return default;
            }

            try
            {
                var gameStatsEntry = await _context.GameStatistics
                    .Where(e => e.Id == userId && e.GameId == gameId)
                    .FirstOrDefaultAsync();
                _logger.LogInformation("GameStatsEntry: {gameStatsEntry}", gameStatsEntry);

                if (gameStatsEntry == null)
                {
                    _logger.LogError("GameStatsEntry is null");
                    return null;
                }

                var stats = new T
                {
                    RecentScore = gameStatsEntry.RecentScore
                };

                // Populate game-specific fields
                if (stats is VisualMemoryStats visualMemoryStats)
                {
                    if (gameStatsEntry.Mistakes == null)
                    {
                        _logger.LogError("Mistakes is null");
                        return null;
                    }
                    visualMemoryStats.GameMistakes = (int)gameStatsEntry.Mistakes;
                }
                else if (stats is SimonSaysStats simonSaysStats)
                {
                    if (gameStatsEntry.TimePlayed == null)
                    {
                        _logger.LogError("TimePlayed is null");
                        return null;
                    }
                    simonSaysStats.TimePlayed = (TimeSpan)gameStatsEntry.TimePlayed;
                }

                return stats;
            }
            catch (Exception e) when (e is OperationCanceledException || e is ArgumentNullException)
            {
                _logger.LogError("{error}", e.Message);
            }
            _logger.LogError("Failed to fetch user stats for game {gameId}", gameId);
            return null;
        }

        // Save Stats for a Specific User and Game
        public async Task SaveUserStatsAsync(AvailableGames gameId, ClaimsPrincipal user, T stats)
        {
            _logger.LogInformation("Saving stats for game {gameId}", gameId);
            var userId = _userManager.GetUserId(user);

            if (userId == null)
            {
                return;
                _logger.LogError("User ID is null");
            }
            GameStatisticsEntry? entry = null;
            if (stats is VisualMemoryStats visualMemoryStats)
            {
                entry = new GameStatisticsEntry()
                {
                    Id = userId,
                    GameId = gameId,
                    RecentScore = visualMemoryStats.RecentScore,
                    Mistakes = visualMemoryStats.GameMistakes
                };
                
            }
            else if (stats is SimonSaysStats simonSaysStats)
            {
                entry = new GameStatisticsEntry()
                {
                    Id = userId,
                    GameId = gameId,
                    RecentScore = simonSaysStats.RecentScore,
                    TimePlayed = simonSaysStats.TimePlayed
                };
            }

            if (entry == null)
            {
                return;
            }

            try
            {
               if (EntryExists(userId, entry.GameId))
                {
                    _context.Entry(entry).State = EntityState.Modified;
                }
                else
                {
                    _context.Entry(entry).State = EntityState.Added;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("{error}", e.Message);
                return;
            }
            _context.GameStatistics.Add(entry);
                
        }
        private bool EntryExists(string id, AvailableGames gameId)
        {
            return _context.GameStatistics.Any(e => e.GameId == gameId && e.Id == id);
        }
    }
}