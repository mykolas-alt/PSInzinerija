using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using PSInzinerija1.Services;
using PSInzinerija1.Games.SimonSays;


namespace PSInzinerija1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameRulesController : BaseController
    {

        private readonly GameRulesAPIService _gameRulesService;

        public GameRulesController(GameRulesAPIService gameRulesService)
        {
            _gameRulesService = gameRulesService;
        }

        [HttpGet]
        public async Task<ActionResult<GameInfo>> GetRulesAsync()
        {
            var gameInfo = await _gameRulesService.GetGameRulesAsync();
            return HandleResponse(gameInfo, "Game rules not found.");
        }
    }
}
