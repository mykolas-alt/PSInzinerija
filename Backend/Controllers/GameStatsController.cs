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

    [HttpGet("recentscore")]
    public async Task<IActionResult> GetRecentScore()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var highScoreEntry = await _context.HighScores.FirstOrDefaultAsync(h => h.Id == userId);
        if (highScoreEntry == null) return NotFound();

        return Ok(highScoreEntry.RecentScores);
    }

    [HttpPost("recentscore")]
    public async Task<IActionResult> SaveRecentScore([FromBody] int recentScore)
    {
        if (recentScore.Length != 3) return BadRequest("Exactly 3 scores are required.");

        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var highScoreEntry = await _context.HighScores.FirstOrDefaultAsync(h => h.Id == userId);
        if (highScoreEntry == null)
        {
            highScoreEntry = new HighScoresEntry { Id = userId, RecentScore = recentScore };
            _context.HighScores.Add(highScoreEntry);
        }
        else
        {
            highScoreEntry.RecentScores = recentScore;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}
