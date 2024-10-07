using PSInzinerija1.Games.SimonSays;
using PSInzinerija1.Games.SimonSays.Models;

namespace PSInzinerija1.Components.Pages.SimonSays
{
    public partial class SimonSays 
    {
        private SimonSaysManager gameManager = default!;
        public List<Button> Buttons => gameManager.Buttons!;
        protected override async Task OnInitializedAsync()
        {
            gameManager = new SimonSaysManager
            {
                StateHasChanged = StateHasChanged
            };
            await gameManager.StartNewGame();
        }
    }
}
