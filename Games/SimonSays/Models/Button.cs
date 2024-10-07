namespace PSInzinerija1.Games.SimonSays.Models
{
    public class Button
    {
        public string colorClass { get; set; } = "buttonDefault";
        public string Text { get; set; }
        public int Index { get; set; }
        private readonly SimonSaysManager gameInstance;

        public Button(string buttonText, int index, SimonSaysManager game)
        {
            Text = buttonText;
            Index = index;
            gameInstance = game;
        }

        public async Task OnClick(Action buttonPressed)
        {
            if (gameInstance.IsShowingSequence || gameInstance.GameOver)
                return;

            colorClass += " pressed valid";
            buttonPressed.Invoke();
            await Task.Delay(100);
            colorClass = "buttonDefault";
            buttonPressed.Invoke();
            await gameInstance.HandleTileClick(Index - 1);
        }

        public async Task FlashButton(Action colorChanged)
        {
            colorClass += " pressed valid";
            colorChanged.Invoke();
            await Task.Delay(400);
            colorClass = "buttonDefault";
            colorChanged.Invoke();
        }
    }
}
