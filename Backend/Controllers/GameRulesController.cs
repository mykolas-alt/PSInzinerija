using Backend.Services;

using Microsoft.AspNetCore.Mvc;

using PSInzinerija1.Shared.Data.Models;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameRulesController : ControllerBase
    {
        private readonly GameRulesService _gameRulesService;

        public GameRulesController(GameRulesService gameRulesService)
        {
            _gameRulesService = gameRulesService ?? throw new ArgumentNullException(nameof(gameRulesService));
        }

        [HttpGet]
        public async Task<ActionResult<GameInfo>> GetRulesAsync()
        {
            // is yra pattern matching, kuris patikrina ar rules yra ne tuscias
            //grazina 404 jei taisykles nerandamos
            return await _gameRulesService.GetGameRulesAsync() is { Rules.Length: > 0 } gameInfo
                ? Ok(gameInfo)
                : NotFound("Game rules not found.");
        }
    }
}
