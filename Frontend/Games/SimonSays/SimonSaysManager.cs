using System.Text.Json;
using System.Text.Json.Nodes;

using PSInzinerija1.Enums;
using PSInzinerija1.Games.SimonSays.Models;

namespace PSInzinerija1.Games.SimonSays
{
    public class SimonSaysManager : IGameManager
    {
        public record SimonSaysStats(int HighScore, int[] RecentScores, int[] GameMistakes, int[] FastestTimes);
        public List<int> Sequence { get; private set; } = new List<int>();
        public int Level { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;
        public List<int> PlayerInput { get; private set; } = new List<int>();
        public bool GameOver { get; private set; } = true;
        public List<Button> Buttons { get; private set; }
        public bool IsShowingSequence { get; set; } = false;
        private readonly Random rand = new Random();

        public event Action OnStatisticsChanged;

        public Action? OnStateChanged { get; set; }
        public bool IsDisabled { get; set; } = false;

        public AvailableGames GameID => AvailableGames.SimonSays;

        public int[] RecentScores = new int[3];
        public int[] FastestTimes { get; private set; } = new int[3];

        public string SerializedStatistics
        {
            get
            {
                var obj = new SimonSaysStats(HighScore, RecentScores, FastestTimes);
                var json = JsonSerializer.Serialize(obj);

                return json.ToString();
            }
        }

        public SimonSaysManager()
        {
            Buttons = Enumerable.Range(1, 9)
                .Select(index => new Button("", index, this))
                .ToList();
        }

        public async Task StartNewGame()
        {
            Level = 0;
            Sequence.Clear();
            PlayerInput.Clear();
            GameOver = false;
            await GenerateSequence();
        }

        private async Task GenerateSequence()
        {
            Sequence.Add(rand.Next(1, 10));
            await ShowSequence();
        }

        private async Task ShowSequence()
        {
            IsShowingSequence = true;

            foreach (int index in Sequence)
            {
                var button = Buttons[index - 1];
                int levelBasedDelay = Math.Max(200 - (Level * 10), 50);
                int levelBasedFlash = Math.Max(400 - (Level * 20), 100);
                await button.FlashButton(OnStateChanged, delayBeforeFlash: levelBasedDelay, duration: levelBasedFlash);
            }
            IsShowingSequence = false;
        }

        public async Task HandleTileClick(int tileIndex)
        {
            if (GameOver) return;

            int playerInputTile = tileIndex + 1;
            PlayerInput.Add(playerInputTile);

            if (!IsInputCorrect())
            {
                if (Level > HighScore)
                {
                    HighScore = Level;
                    OnStatisticsChanged?.Invoke();
                }
                GameOver = true;
                Level = 0;
                IsDisabled = false;
                return;
            }

            if (PlayerInput.Count == Sequence.Count)
            {
                Level++;
                await Task.Delay(200);
                PlayerInput.Clear();
                await GenerateSequence();
            }
        }

        private bool IsInputCorrect()
        {
            int currentInputIndex = PlayerInput.Count - 1;
            return PlayerInput[currentInputIndex] == Sequence[currentInputIndex];
        }

        public void LoadStatisticsFromJSON(string? json)
        {
            if (json == null)
            {
                return;
            }

            SimonSaysStats? stats = JsonSerializer.Deserialize<SimonSaysStats>(json);
            if(stats != null)
            {
                if (stats?.HighScore != null && stats?.HighScore > HighScore)
                {
                    HighScore = stats.HighScore;
                }
                
                if(stats?.RecentScores != null)
                {
                    RecentScores = stats.RecentScores;
                }

                if(stats?.GameMistakes != null)
                {
                    GameMistakes = stats.GameMistakes;
                }

                if(stats?.FastestTimes != null)
                {
                    FastestTimes = stats.FastestTimes;
                }
            }
        }

        public bool SetHighScore(int? highScore)
        {
            if (highScore == null || highScore.Value < HighScore)
            {
                return false;
            }

            HighScore = highScore.Value;
            return true;
        }

        private void UpdateRecentScores(int latestScore)
        {
            // Shift the scores to the left
            for (int i = 0; i < RecentScores.Length - 1; i++)
            {
                RecentScores[i+1] = RecentScores[i];
            }
            // Add the latest score to the end
            RecentScores[0] = latestScore;
        }
    }
}