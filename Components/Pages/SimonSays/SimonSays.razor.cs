using PSInzinerija1.Games.SimonSays;
using PSInzinerija1.Games.SimonSays.Models;
using PSInzinerija1.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;

namespace PSInzinerija1.Components.Pages.SimonSays
{
    public partial class SimonSays 
    {
        private readonly SimonSaysManager gameManager = new SimonSaysManager();
        protected override async Task OnInitializedAsync()
        {
            gameManager.OnStateChanged = StateHasChanged;
            //await gameManager.StartNewGame();
        }
    }
}

