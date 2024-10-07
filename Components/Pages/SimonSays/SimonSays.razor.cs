using PSInzinerija1.Games.SimonSays;
using PSInzinerija1.Games.SimonSays.Models;
using PSInzinerija1.Services;

namespace PSInzinerija1.Components.Pages.SimonSays
{
   public partial class SimonSays 
    {
        private readonly SimonSaysManager gameManager = new SimonSaysManager();
        public List<Button> Buttons => gameManager.Buttons!;
        public int HighScore  => gameManager.HighScore;
        public int Level => gameManager.Level - 1;
        
        protected override async Task OnInitializedAsync()
        {
            gameManager.OnStateChanged = StateHasChanged;
            await gameManager.StartNewGame();
        }
    }
}

