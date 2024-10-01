using Microsoft.AspNetCore.Authentication;

namespace PSInzinerija1.Components.Pages.SimonSays
{
    public partial class SimonSays
    {
        public List<int> Sequence { get; private set; } = new List<int>();
        public int Level { get; private set; } = 0;
        public List<int> PlayerInput { get; private set; } = new List<int>();
        public bool GameOver { get; private set; } = false;
        protected List<Button> Buttons { get; private set; }
        public bool IsShowingSequence { get; private set; } = false;
        private readonly Random rand = new Random();
        private readonly SimonSays game;
        public SimonSays()
        {
            game = this;
            Buttons = Enumerable.Range(1, 9)
                .Select(index => new Button(index.ToString(), index, game))
                .ToList();
        }

        public class Button
        {
            public string Color { get; set; } = "white";
            public string Text { get; set; }
            public int Index { get; set; }

            private readonly SimonSays gameInstance;

            public Button(string buttonText, int index, SimonSays game)
            {
                Text = buttonText;
                Index = index;
                gameInstance = game;
            }

            public async Task OnClick(Action cb2)
            {
                if (gameInstance.IsShowingSequence || gameInstance.GameOver)
                    return;

                Color = "blue";
                cb2.Invoke();
                await Task.Delay(100);
                Color = "white";
                cb2.Invoke();
                await gameInstance.HandleTileClick(Index - 1);
            }

            public async Task FlashButton(Action cb)
            {
                Color = "blue";
                cb.Invoke();
                await Task.Delay(400);
                Color = "white";
                cb.Invoke();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await game.StartNewGame();
        }

        public async Task StartNewGame()
        {
            Level = 1;
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
                await button.FlashButton(StateHasChanged);
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
                GameOver = true;
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
