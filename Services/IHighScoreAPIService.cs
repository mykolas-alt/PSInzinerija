using PSInzinerija1.Enums;
using PSInzinerija1.Models;

namespace PSInzinerija1.Services
{
    public interface IHighScoreAPIService
    {
        public Task<int?> GetHighScoreAsync(AvailableGames gameId, long userId);
        public Task<bool> SaveHighScoreToDbAsync(HighScoresEntry model);
    }
}