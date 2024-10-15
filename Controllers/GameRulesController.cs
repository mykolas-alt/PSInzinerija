using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using PSInzinerija1.Services;
using PSInzinerija1.Games.SimonSays;


namespace PSInzinerija1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameRulesController : ControllerBase
    {
        
        private readonly GameRulesAPIService _gameRulesService;

        public GameRulesController(GameRulesAPIService gameRulesService)
        {
            _gameRulesService = gameRulesService;
        }

        [HttpGet]
        public async Task<ActionResult<GameInfoStruct>> GetRulesAsync()
        {
            return await _gameRulesService.GetGameRulesAsync() is { rules: { Length: > 0 } } gameInfo // is yra pattern matching, kuris patikrina ar rules yra ne tuscias
                ? Ok(gameInfo)
                : NotFound("Game rules not found."); //grazina 404 jei taisykles nerandamos
        }
    }
}
