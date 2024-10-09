using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PSInzinerija1.Data.ApplicationDbContext;
using PSInzinerija1.Enums;
using PSInzinerija1.Data.Models;

namespace PSInzinerija1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HighScoresController(ApplicationDbContext context, UserManager<User> userManager) : ControllerBase
    {
        /// <summary>
        /// Gets all high score entries for a specific game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <returns>A list of high score entries</returns>
        [HttpGet("{game}/all")]
        public async Task<ActionResult<IEnumerable<LeaderboardEntry>>> GetGameHighScores(AvailableGames game)
        {
            return await context.HighScores
                .Where(e => e.GameId == game)
                .Join(context.Users, e => e.Id, u => u.Id, (e, u) =>
                    new LeaderboardEntry(u.UserName ?? "Anon", e.HighScore, e.RecordDate))
                .ToListAsync();
        }

        /// <summary>
        /// Gets all high score entries for all available games
        /// </summary>
        /// <returns>A list of high score entries</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HighScoresEntry>>> GetAllHighScores()
        {
            return await context.HighScores.ToListAsync();
        }

        /// <summary>
        /// Gets the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <returns>High score entry</returns>
        [Authorize]
        [HttpGet("{game}")]
        public async Task<ActionResult<HighScoresEntry>> GetUserHighScore(AvailableGames game)
        {
            var user_id = userManager.GetUserId(HttpContext.User);

            var todoItem = await context.HighScores
                .Where(m => m.GameId == game && m.Id == user_id)
                .SingleOrDefaultAsync();

            return todoItem == null ? NotFound() : todoItem;
        }

        /// <summary>
        /// Updates or sets the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <param name="newHighScore">The new high score</param>
        [Authorize]
        [HttpPut("{game}")]
        public async Task<IActionResult> PutUserHighScore(AvailableGames game, [FromBody] int newHighScore)
        {
            var user_id = userManager.GetUserId(HttpContext.User);
            if (user_id == null)
            {
                return BadRequest();
            }

            var entry = new HighScoresEntry()
            {
                GameId = game,
                Id = user_id,
                HighScore = newHighScore,
                RecordDate = DateTime.UtcNow
            };

            context.Entry(entry).State = EntryExists(user_id, entry.GameId) ?
                EntityState.Modified : EntityState.Added;

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        [Authorize]
        [HttpDelete("{game}")]
        public async Task<IActionResult> DeleteUserHighScore(AvailableGames game)
        {
            var user_id = userManager.GetUserId(HttpContext.User);
            var todoItem = await context.HighScores.FindAsync(user_id, game);
            if (todoItem == null)
            {
                return NotFound();
            }

            context.HighScores.Remove(todoItem);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntryExists(string id, AvailableGames gameId)
        {
            return context.HighScores.Any(e => e.GameId == gameId && e.Id == id);
        }
    }
}