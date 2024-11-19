using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Diagnostics;


using Frontend.Games.SimonSays.Models;
using Shared.Enums;
using Microsoft.VisualBasic;



namespace Frontend.Games.SimonSays
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
        public TimeSpan TimePlayed;
        private readonly Stopwatch timer = new Stopwatch();


        // public string SerializedStatistics
        // {
        //     get
        //     {
        //         var specificStats = new SimonSaysStats { TimePlayed = TimePlayed };
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
            timer.Start();
        }

        private async Task GenerateSequence()
        {
            Sequence.Add(rand.Next(1, 10));
            await ShowSequence();
        }

        private async Task ShowSequence()
        {
            timer.Stop();
            IsShowingSequence = true;

            foreach (int index in Sequence)
            {
                var button = Buttons[index - 1];
                int levelBasedDelay = Math.Max(200 - Level * 10, 50);
                int levelBasedFlash = Math.Max(400 - Level * 20, 100);
                await button.FlashButton(OnStateChanged, delayBeforeFlash: levelBasedDelay, duration: levelBasedFlash);
            }
            IsShowingSequence = false;
            timer.Start();
        }

        public async Task HandleTileClick(int tileIndex)
        {
            if (GameOver) return;

            int playerInputTile = tileIndex + 1;
            PlayerInput.Add(playerInputTile);

            if (!IsInputCorrect())
            {
                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;
                RecentScore = Level;
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
                RecentScore = Level;
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

        public bool SetHighScore(int? highScore)
        {
            if (highScore == null || highScore.Value < HighScore)
            {
                return false;
            }

            HighScore = highScore.Value;
            return true;
        }
    }
}