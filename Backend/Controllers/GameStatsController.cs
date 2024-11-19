using Microsoft.AspNetCore.Mvc;
using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;
using Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using PSInzinerija1.Shared.Data.Models.Stats;
using Backend.Services;

[ApiController]
[Route("api/[controller]")]
public class GameStatsController : ControllerBase
{
    private readonly GameStatsService<VisualMemoryStats> _visualMemoryStatsService;
    private readonly GameStatsService<SimonSaysStats> _simonSaysStatsService;

    public GameStatsController(IServiceProvider serviceProvider)
    {
        _visualMemoryStatsService = serviceProvider.GetRequiredService<GameStatsService<VisualMemoryStats>>();
        _simonSaysStatsService = serviceProvider.GetRequiredService<GameStatsService<SimonSaysStats>>();
    }
    
    [Authorize]
    [HttpGet("{game}/stats")]
    public async Task<ActionResult<GameStats>> GetStats(AvailableGames game)
    {
        if(game == AvailableGames.VisualMemory)
        {
            var stats = await _visualMemoryStatsService.GetUserStatsAsync(AvailableGames.VisualMemory, HttpContext.User);
            if (stats == null) return NotFound();
            return Ok(stats);
        }
        else if(game == AvailableGames.SimonSays)
        {
            var stats = await _simonSaysStatsService.GetUserStatsAsync(AvailableGames.SimonSays, HttpContext.User);
            if (stats == null) return NotFound();
            return Ok(stats);
        }

        return BadRequest("Invalid game");
    }

    [Authorize]
    [HttpPost("{game}/stats")]
    public async Task<ActionResult> SaveStats(AvailableGames game)
    {
        
        if(game == AvailableGames.VisualMemory )
        {
            var stats = await Request.ReadFromJsonAsync<VisualMemoryStats>();
            if(stats == null) return BadRequest("Invalid stats");
            await _visualMemoryStatsService.SaveUserStatsAsync(game, HttpContext.User, stats);
            return Ok();
        }
        else if(game == AvailableGames.SimonSays)
        {
            var stats = await Request.ReadFromJsonAsync<SimonSaysStats>();
            if(stats == null) return BadRequest("Invalid stats");
            await _simonSaysStatsService.SaveUserStatsAsync(game, HttpContext.User, stats);
            return Ok();
        }
        return BadRequest("Invalid game");
    }
}