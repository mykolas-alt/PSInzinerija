namespace PSInzinerija1.Components.Pages.SimonSays
{
    public partial class SimonSays
    {
        protected List<Button> Buttons { get; } = Enumerable.Range(1, 9)
           .Select(index => new Button(index.ToString()))
           .ToList();

        public class Button(string buttonText)
        {
            public string Color { get; set; } = "white";
            public string Text { get; set; } = buttonText;

            public async Task OnClick()
            {
                Color = "blue";
                await Task.Delay(100);
                Color = "white";
            }
        }
    }
}