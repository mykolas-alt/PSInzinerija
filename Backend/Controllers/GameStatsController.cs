using Microsoft.AspNetCore.Mvc;
using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;
using Shared.Enums;

[ApiController]
[Route("api/[controller]")]
public class GameStatsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GameStatsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    [HttpGet("{game}/recentscore")]
    public async Task<ActionResult> GetRecentScore()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var gameStatsEntry = await _context.GameStatistics.FirstOrDefaultAsync(h => h.Id == userId);
        if (gameStatsEntry == null) return NotFound();

        return Ok(gameStatsEntry.RecentScore);
    }
    [HttpPost("{game}/recentscore")]
    public async Task<ActionResult> SaveRecentScore([FromBody] int recentScore, AvailableGames game)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var gameStatsEntry = await _context.GameStatistics.FirstOrDefaultAsync(h => h.Id == userId);
        if (gameStatsEntry == null)
        {
            gameStatsEntry = new GameStatisticsEntry() { Id = userId, GameId = game, RecentScore = recentScore };
            _context.GameStatistics.Add(gameStatsEntry);
        }
        else
        {
            gameStatsEntry.RecentScore = recentScore;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpGet("{game}/mistakes")]
    public async Task<ActionResult> GetGameMistakes()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var gameStatsEntry = await _context.GameStatistics.FirstOrDefaultAsync(h => h.Id == userId);
        if (gameStatsEntry == null) return NotFound();

        return Ok(gameStatsEntry.GameMistakes);
    }
    [HttpPost("{game}/mistakes")]
    public async Task<ActionResult> SaveGameMistakes([FromBody] int gameMistakes, AvailableGames game)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var gameStatsEntry = await _context.GameStatistics.FirstOrDefaultAsync(h => h.Id == userId);
        if (gameStatsEntry == null)
        {
            gameStatsEntry = new GameStatisticsEntry() { Id = userId, GameId = game, Mistakes = gameMistakes };
            _context.GameStatistics.Add(gameStatsEntry);
        }
        else
        {
            gameStatsEntry.Mistakes = gameMistakes;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpGet("{game}/timeplayed")]
    public async Task<ActionResult> GetTimePlayed()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var gameStatsEntry = await _context.GameStatistics.FirstOrDefaultAsync(h => h.Id == userId);
        if (gameStatsEntry == null) return NotFound();

        return Ok(gameStatsEntry.TimePlayed);
    }
    [HttpPost("{game}/timeplayed")]
    public async Task<ActionResult> SaveTimePlayed([FromBody] TimeSpan timePlayed, AvailableGames game)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var gameStatsEntry = await _context.GameStatistics.FirstOrDefaultAsync(h => h.Id == userId);
        if (gameStatsEntry == null)
        {
            gameStatsEntry = new GameStatisticsEntry() { Id = userId, GameId = game, TimePlayed = timePlayed };
            _context.GameStatistics.Add(gameStatsEntry);
        }
        else
        {
            gameStatsEntry.TimePlayed = timePlayed;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}