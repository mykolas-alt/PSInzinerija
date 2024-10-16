using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PSInzinerija1.Data.ApplicationDbContext;
using PSInzinerija1.Data.Models;
using PSInzinerija1.Enums;

namespace PSInzinerija1.Services
{
    public class HighScoreService(ApplicationDbContext context, UserManager<User> userManager)
    {
        public async Task<List<LeaderboardEntry>?> GetGameHighScoresAsync(AvailableGames game)
        {
            try
            {
                var query = from a in context.HighScores.Where(e => e.GameId == game)
                            join b in context.Users
                                on a.Id equals b.Id
                            orderby a.HighScore descending
                            select new LeaderboardEntry(b.UserName ?? "Anon", a.HighScore, a.RecordDate);

                var res = await query.ToListAsync();

                return res;
            }
            catch (Exception e) when (e is OperationCanceledException || e is ArgumentNullException)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<List<HighScoresEntry>?> GetAllHighScoresAsync()
        {
            try
            {
                return await context.HighScores.OrderByDescending(e => e.HighScore).ToListAsync();
            }
            catch (Exception e) when (e is OperationCanceledException || e is ArgumentNullException)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<HighScoresEntry?> GetUserHighScoreAsync(AvailableGames game, ClaimsPrincipal claims)
        {
            var user_id = userManager.GetUserId(claims);

            if (user_id == null)
            {
                return null;
            }

            try
            {
                var todoItem = await context.HighScores
                    .Where(m => m.GameId == game && m.Id == user_id)
                    .SingleOrDefaultAsync();

                return todoItem;
            }
            catch (Exception e) when (e is OperationCanceledException || e is ArgumentNullException || e is InvalidOperationException)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<bool> PutUserHighScoreAsync(AvailableGames game, int newHighScore, ClaimsPrincipal claims)
        {
            var user_id = userManager.GetUserId(claims);
            if (user_id == null)
            {
                return false;
            }

            var entry = new HighScoresEntry()
            {
                GameId = game,
                Id = user_id,
                HighScore = newHighScore,
                RecordDate = DateTime.UtcNow
            };

            try
            {
                if (EntryExists(user_id, entry.GameId))
                {
                    context.Entry(entry).State = EntityState.Modified;
                }
                else
                {
                    context.Entry(entry).State = EntityState.Added;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteUserHighScoreAsync(AvailableGames game, ClaimsPrincipal claims)
        {
            var user_id = userManager.GetUserId(claims);
            var todoItem = await context.HighScores.FindAsync(user_id, game);
            if (todoItem == null)
            {
                return false;
            }

            try
            {
                context.HighScores.Remove(todoItem);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private bool EntryExists(string id, AvailableGames gameId)
        {
            return context.HighScores.Any(e => e.GameId == gameId && e.Id == id);
        }
    }
}