using System.Text.Json;

using Frontend.Games.VisualMemory.Models;

using Shared.Enums;
using PSInzinerija1.Shared.Data.Models;
using PSInzinerija1.Shared.Data.Models.Stats;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Games.VisualMemory
{
    public class VisualMemoryManager : IGameManager
    {
        public record VisualMemoryHighScore(int HighScore);
        public int Score { get; private set; } = 0;
        public int GameMistakes { get; set; } = 0;
        public int RecentScore {get; set;} = 0;
        public int HighScore { get; private set; } = 0;
        public Pattern Pattern { get; private set; } = new();
        public AvailableGames GameID => AvailableGames.VisualMemory;
        public string SerializedStatistics
        {
            get
            {
                var obj = new VisualMemoryHighScore(HighScore);
                var json = JsonSerializer.Serialize(obj);

                return json.ToString();
            }
        }

        public event Action? OnStatisticsChanged;
        public event Action? OnScoreChanged;

        private readonly int _roundStartDelay = 1500;
        private int _mistakeCount = 0;
        private int _correctCount = 0;
        

        public async Task StartNewGame()
        {

            Score = 0;
            GameMistakes = 0;
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
                GameMistakes++;
            }
            else
            {
                _correctCount++;
            }

            if (_mistakeCount >= 3)
            {
                // game over
                RecentScore = Score;
                OnScoreChanged?.Invoke();
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
        

        private void ResetRound()
        {
            _correctCount = 0;
            _mistakeCount = 0;
            GameMistakes = 0;
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

            VisualMemoryHighScore? score = JsonSerializer.Deserialize<VisualMemoryHighScore>(json);

            if (score?.HighScore != null && score?.HighScore > HighScore)
            {
                HighScore = score.HighScore;
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