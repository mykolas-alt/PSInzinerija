using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PSInzinerija1.Enums;
using PSInzinerija1.Models;
using PSInzinerija1.Services.ApplicationDbContext;

namespace PSInzinerija1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HighScoresController(AppDbContext context) : ControllerBase
    {
        /// <summary>
        /// Gets all high score entries for a specific game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <returns>A list of high score entries</returns>
        [HttpGet("{game}")]
        public async Task<ActionResult<IEnumerable<HighScoresEntry>>> GetGameHighScores(AvailableGames game)
        {
            return await context.HighScores
                .Where(e => e.GameId == game)
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
        /// <param name="user_id">The ID of the user that the entry belongs to</param>
        /// <returns>High score entry</returns>
        [HttpGet("{game}/{user_id}")]
        public async Task<ActionResult<HighScoresEntry>> GetUserHighScore(AvailableGames game, long user_id)
        {
            var todoItem = await context.HighScores
                .Where(m => m.GameId == game && m.Id == user_id)
                .SingleOrDefaultAsync();

            return todoItem == null ? NotFound() : todoItem;
        }

        /// <summary>
        /// Updates or sets the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <param name="user_id">The ID of the user that the entry belongs to</param>
        /// <param name="entry">The entry to be updated or set</param>
        [HttpPut("{game}/{user_id}")]
        public async Task<IActionResult> PutUserHighScore(AvailableGames game, long user_id, HighScoresEntry entry)
        {
            if (user_id != entry.Id || game != entry.GameId)
            {
                return BadRequest();
            }

            context.Entry(entry).State = EntryExists(user_id, entry.GameId) ?
                EntityState.Modified : EntityState.Added;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryExists(user_id, entry.GameId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes the high score entry for a specific user and game
        /// </summary>
        /// <param name="game">One of the available games</param>
        /// <param name="user_id">The ID of the user that the entry belongs to</param>
        [HttpDelete("{game}/{user_id}")]
        public async Task<IActionResult> DeleteUserHighScore(AvailableGames game, long user_id)
        {
            var todoItem = await context.HighScores.FindAsync(user_id, game);
            if (todoItem == null)
            {
                return NotFound();
            }

            context.HighScores.Remove(todoItem);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntryExists(long id, AvailableGames gameId)
        {
            return context.HighScores.Any(e => e.GameId == gameId && e.Id == id);
        }
    }
}