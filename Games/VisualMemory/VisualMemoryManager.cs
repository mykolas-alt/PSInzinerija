using PSInzinerija1.Games.VisualMemory.Models;

namespace PSInzinerija1.Games.VisualMemory
{
    public class VisualMemoryManager
    {
        private readonly int _roundStartDelay = 1000;
        public int Score { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;
        private int _mistakeCount = 0;
        private int _correctCount = 0;
        public Pattern Pattern { get; private set; } = new();

        public async Task StartNewGame()
        {
            Score = 0;
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
            }
            else
            {
                _correctCount++;
            }

            if (_mistakeCount >= 3)
            {
                // game over
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
            }
        }
        private void ResetRound()
        {
            _correctCount = 0;
            _mistakeCount = 0;
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
    }
}