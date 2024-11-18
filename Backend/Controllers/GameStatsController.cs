[ApiController]
[Route("api/gamestats")]
public class GameStatsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GameStatsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("recentscores")]
    public async Task<IActionResult> GetRecentScores()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var highScoreEntry = await _context.HighScores.FirstOrDefaultAsync(h => h.Id == userId);
        if (highScoreEntry == null) return NotFound();

        return Ok(highScoreEntry.RecentScores);
    }

    [HttpPost("recentscores")]
    public async Task<IActionResult> SaveRecentScores([FromBody] int[] recentScores)
    {
        if (recentScores.Length != 3) return BadRequest("Exactly 3 scores are required.");

        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var highScoreEntry = await _context.HighScores.FirstOrDefaultAsync(h => h.Id == userId);
        if (highScoreEntry == null)
        {
            highScoreEntry = new HighScoresEntry { Id = userId, RecentScores = recentScores };
            _context.HighScores.Add(highScoreEntry);
        }
        else
        {
            highScoreEntry.RecentScores = recentScores;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}
