namespace PSInzinerija1.Games
{
    public interface IGameManager
    {
        public string GameID { get; }
        public string SerializedStatistics { get; }

        public event Action OnStatisticsChanged;

        public void LoadStatisticsFromJSON(string? json);
    }
}