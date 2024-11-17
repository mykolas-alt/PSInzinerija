using Microsoft.AspNetCore.Mvc;

using PSInzinerija1.Services;
using PSInzinerija1.Shared.Data.Models;


namespace PSInzinerija1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameStatsController : ControllerBase
    {
        private readonly GameStatsService _gameStatsService;
        public GameStatsController(GameStatsService gameStatsService)
        {
            _gameStatsService = gameStatsService;
        }
        [HttpGet("{game}")]
        public async Task<ActionResult<Stats>> GetStatsAsync(AvailableGames game)
        {
            var stats = await gameStatsService.GetStatsAsync(HttpContext.User, game);

            return stats == null ? NotFound() : Ok(stats);
        }
    }
}