using PSInzinerija1.Games.SimonSays.Models;

namespace PSInzinerija1.Games.SimonSays
{
    public class SimonSaysManager
    {
        public List<int> Sequence { get; private set; } = new List<int>();
        public int Level { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;
        public List<int> PlayerInput { get; private set; } = new List<int>();
        public bool GameOver { get; private set; } = false;
        public List<Button> Buttons { get; private set; }
        public bool IsShowingSequence { get; private set; } = false;
        private readonly Random rand = new Random();
        public Action? OnStateChanged { get; set; }

        public SimonSaysManager()
        {
            Buttons = Enumerable.Range(1, 9)
                .Select(index => new Button(index.ToString(), index, this))
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
                var button = Buttons[index - 1]; // adjusting for 0-based indexing
                await button.FlashButton(OnStateChanged);
                await Task.Delay(200);
            }
            IsShowingSequence = false;
        }

        public async Task HandleTileClick(int tileIndex)
        {
            if (GameOver) return;

            int playerInputTile = tileIndex + 1; // +1, because 0-based indexing
            PlayerInput.Add(playerInputTile);

            if (!IsInputCorrect())
            {
                if (Level > HighScore)
                {
                    HighScore = Level;
                }
                GameOver = true;
                await StartNewGame();
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
            if (currentInputIndex >= Sequence.Count) return false;
            return PlayerInput[currentInputIndex] == Sequence[currentInputIndex];
        }
    }
}
