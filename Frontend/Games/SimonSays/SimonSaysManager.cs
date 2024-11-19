using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json;


using PSInzinerija1.Enums;
using PSInzinerija1.Games.SimonSays.Models;
// using PSInzinerija1.Shared.Data.Models;


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

        public event Action? OnStatisticsChanged;

        public Action? OnStateChanged { get; set; }
        public bool IsDisabled { get; set; } = false;

        public AvailableGames GameID => AvailableGames.SimonSays;

        public int RecentScore { get; set; } = 0;
        public int MostRecentTime;

        // public string SerializedStatistics
        // {
        //     get
        //     {
        //         var specificStats = new SimonSaysStats { MostRecentTime = MostRecentTime };
        //         var gameStats = new GameStats<SimonSaysStats>(specificStats)
        //         {
        //             HighScore = HighScore,
        //             RecentScores = RecentScores
        //         };

        //         var json = JsonSerializer.Serialize(gameStats);

        //         return json.ToString();
        //     }
        // }
        public string SerializedStatistics
        {
            get
            {
                var obj = new
                {
                    HighScore
                };
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
                
                var updatedStats = new SimonSaysStats
                {
                    HighScore = HighScore,
                    RecentScore = RecentScore,
                    MostRecentTime = MostRecentTime
                };

                await SaveStatsAsync(updatedStats);

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

            if (jsonObject != null && jsonObject[nameof(HighScore)] != null)
            {
                HighScore = jsonObject[nameof(HighScore)].Deserialize<int>();
            }

        }

        public bool SetHighScore(int? HighScore)
        {
            if (HighScore == null || HighScore.Value < HighScore)
            {
                return false;
            }

            HighScore = HighScore.Value;
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

        // private async Task SaveStatsAsync(SimonSaysStats stats)
        // {
        //     // Replace "user123" with logic to get the current user's ID
        //     var userId = "user123"; // Replace with actual user retrieval
        //     await GameStatsService.SaveStatsAsync(userId, AvailableGames.SimonSays, stats);
        // }
    }
}