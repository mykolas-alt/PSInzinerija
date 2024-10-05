using Microsoft.EntityFrameworkCore;

using PSInzinerija1.Models;

namespace PSInzinerija1.Services.ApplicationDbContext
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<HighScoresEntry> HighScores { get; set; }
    }
}