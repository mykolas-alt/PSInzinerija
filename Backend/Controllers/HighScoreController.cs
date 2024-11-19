using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSInzinerija1.Enums;
using PSInzinerija1.Shared.Data.Models;
using Backend.Filters;
using Backend.Data.Models;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [ServiceFilter<APIHitCountFilter>]
    [Route("api/[controller]")]
    public class HighScoresController : ControllerBase
    {
        private readonly HighScoreService _highScoreService;

        public HighScoresController(HighScoreService highScoreService)
        {
            _highScoreService = highScoreService ?? throw new ArgumentNullException(nameof(highScoreService));
        }

        /// <summary>
        /// Gets all high score entries for a specific game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <returns>A list of high score entries</returns>
        [HttpGet("{game}/all")]
        public async Task<ActionResult<IEnumerable<LeaderboardEntry>>> GetGameHighScoresAsync(AvailableGames game)
        {
            var list = await _highScoreService.GetGameHighScoresAsync(game);

            return list == null ? NotFound() : Ok(list);
        }

        /// <summary>
        /// Gets all high score entries for all available games
        /// </summary>
        /// <returns>A list of high score entries</returns>
        [HttpGet]
        public async Task<ActionResult<List<HighScoresEntry>>> GetAllHighScoresAsync()
        {
            var list = await _highScoreService.GetAllHighScoresAsync();

            return list == null ? NotFound() : Ok(list);
        }

        /// <summary>
        /// Gets the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <returns>High score entry</returns>
        [Authorize]
        [HttpGet("{game}")]
        public async Task<ActionResult<HighScoresEntry>> GetUserHighScoreAsync(AvailableGames game)
        {
            var highScore = await _highScoreService.GetUserHighScoreAsync(game, HttpContext.User);

            return highScore == null ? NotFound() : Ok(highScore);
        }

        /// <summary>
        /// Updates or sets the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <param name="newHighScore">The new high score</param>
        [Authorize]
        [HttpPut("{game}")]
        public async Task<ActionResult> PutUserHighScore(AvailableGames game, [FromBody] int newHighScore)
        {
            var success = await _highScoreService.PutUserHighScoreAsync(game, newHighScore, HttpContext.User);

            // TODO: pateikti detalesne informacija
            return success ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Deletes the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        [Authorize]
        [HttpDelete("{game}")]
        public async Task<ActionResult> DeleteUserHighScore(AvailableGames game)
        {
            var success = await _highScoreService.DeleteUserHighScoreAsync(game, HttpContext.User);

            // TODO: pateikti detalesne informacija
            return success ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}