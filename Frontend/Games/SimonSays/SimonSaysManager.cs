using System;
using System.Collections.Generic;
using System.Text.Json;

using PSInzinerija1.Data.Models;
using PSInzinerija1.Enums;
using PSInzinerija1.Games.SimonSays.Models;


namespace PSInzinerija1.Games.SimonSays
{
    public class SimonSaysManager : IGameManager
    {
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
        public int[] GameMistakes = new int[3];
        public int[] FastestTimes = new int[3];

        public string SerializedStatistics
        {
            get
            {
                var specificStats = new SimonSaysStats { FastestTimes = FastestTimes };
                var gameStats = new GameStats<SimonSaysStats>(specificStats)
                {
                    HighScore = HighScore,
                    RecentScores = RecentScores
                };

                var json = JsonSerializer.Serialize(gameStats);

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
                if (Level > SimonSaysStats)
                {
                    SimonSaysStats = Level;
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

            var jsonObject = JsonNode.Parse(json)?.AsObject();

            if (jsonObject != null && jsonObject[nameof(SimonSaysStats)] != null)
            {
                SimonSaysStats = jsonObject[nameof(SimonSaysStats)].Deserialize<int>();
            }

        }

        public bool SetHighScore(int? SimonSaysStats)
        {
            if (SimonSaysStats == null || SimonSaysStats.Value < SimonSaysStats)
            {
                return false;
            }

            SimonSaysStats = SimonSaysStats.Value;
            return true;
        }

        private void UpdateRecentScores(int latestScore)
        {
            for (int i = 0; i < RecentScores.Length - 1; i++)
            {
                RecentScores[i+1] = RecentScores[i];
            }
            RecentScores[0] = latestScore;
        }
    }
}