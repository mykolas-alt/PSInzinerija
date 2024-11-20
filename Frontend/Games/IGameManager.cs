using PSInzinerija1.Enums;

namespace PSInzinerija1.Games
{
    public interface IGameManager
    {
        public int HighScore { get; }
        public AvailableGames GameID { get; }
        public string SerializedStatistics { get; }

        public event Action OnStatisticsChanged;

        public void LoadStatisticsFromJSON(string? json);
        public bool SetHighScore(int? highScore);
    }
}