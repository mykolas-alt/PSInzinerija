using System.Text.Json;

using Microsoft.AspNetCore.SignalR;

using PSInzinerija1.Enums;
using PSInzinerija1.Games.VisualMemory.Models;

namespace PSInzinerija1.Games.VisualMemory
{
    public class VisualMemoryManager : IGameManager
    {
        public record VisualMemoryStats(int HighScore, int[] RecentScores, int[] GameMistakes);
        public int Score { get; private set; } = 0;
        public int[]  GameMistakes { get; private set; } = new int[3];
        public int[] RecentScores {get; set;} = new int[3];
        public int HighScore { get; private set; } = 0;
        public Pattern Pattern { get; private set; } = new();
        public AvailableGames GameID => AvailableGames.VisualMemory;
        public string SerializedStatistics
        {
            get
            {
                var obj = new VisualMemoryStats(HighScore, RecentScores, GameMistakes);
                var json = JsonSerializer.Serialize(obj);

                return json.ToString();
            }
        }

        public event Action? OnStatisticsChanged;

        private readonly int _roundStartDelay = 1500;
        private int _mistakeCount = 0;
        private int _correctCount = 0;
        private int _gameMistakes = 0;

        public async Task StartNewGame()
        {
            Score = 0;
            _gameMistakes = 0;
            Pattern = new();
            ResetRound();
            await BeginRound();
        }

        private void DisableButtons()
        {
            foreach (var cell in Pattern)
            {
                cell.Pressed = true;
            }
        }

        private void EnableButtons()
        {
            foreach (var cell in Pattern)
            {
                cell.Pressed = false;
            }
        }

        public async Task HandleInput(PatternCell buttonSquare)
        {
            if (buttonSquare.Pressed)
            {
                return;
            }
            buttonSquare.Pressed = true;

            if (buttonSquare.Value == PatternValue.Invalid)
            {
                _mistakeCount++;
                _gameMistakes++;
            }
            else
            {
                _correctCount++;
            }

            if (_mistakeCount >= 3)
            {
                // game over
                UpdateRecentScores(Score);
                await StartNewGame();

            }
            else if (_correctCount >= Pattern.ValidCellAmount)
            {
                ResetRound();
                await Advance();
            }

        }

        private void UpdateHighScore()
        {
            
            if (Score > HighScore)
            {
                HighScore = Score;
                OnStatisticsChanged?.Invoke();
            }
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

        private void ResetRound()
        {
            _correctCount = 0;
            _mistakeCount = 0;
            _gameMistakes = 0;
        }

        private async Task Advance()
        {
            Pattern.IncreaseDifficulty();
            Pattern.GenerateNewPattern();
            Score++;
            UpdateHighScore();
            await BeginRound();
        }

        private async Task BeginRound()
        {
            DisableButtons();
            await Task.Delay(_roundStartDelay);
            EnableButtons();
        }

        public void LoadStatisticsFromJSON(string? json)
        {
            if (json == null)
            {
                return;
            }

            VisualMemoryStats? stats = JsonSerializer.Deserialize<VisualMemoryStats>(json);
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