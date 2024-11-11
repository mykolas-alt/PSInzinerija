using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PSInzinerija1.Data.ApplicationDbContext;
using PSInzinerija1.Data.Models;
using PSInzinerija1.Enums;
using System.Security.Claims;

public class HighScoreFetcher<TUser, TGame>
    where TUser : class, IUser
    where TGame : Enum
{
    private readonly ApplicationDbContext context;
    private readonly UserManager<TUser> userManager;

    public HighScoreFetcher(ApplicationDbContext context, UserManager<TUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    public async Task<Dictionary<string, List<HighScoresEntry>>> GetUserRecentScoresAsync(ClaimsPrincipal user, int count)
    {
        var userId = userManager.GetUserId(user);
        if (userId == null)
        {
            return new Dictionary<string, List<HighScoresEntry>>();
        }

        var recentScores = new Dictionary<string, List<HighScoresEntry>>();

        foreach (TGame game in Enum.GetValues(typeof(TGame)))
        {
            var scores = await context.HighScores
                .Where(e => e.GameId.Equals(game) && e.Id == userId)
                .OrderByDescending(e => e.RecordDate)
                .Take(count)
                .ToListAsync();

            recentScores[game.ToString()] = scores;
        }

        return recentScores;
    }
}