using PSInzinerija1.Games.SimonSays;
using PSInzinerija1.Games.SimonSays.Models;
using PSInzinerija1.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;

namespace PSInzinerija1.Components.Pages.SimonSays
{
    public partial class SimonSays 
    {
        
        private readonly SimonSaysManager _gameManager = new SimonSaysManager();
        [Inject]
        public GameRulesAPIService GameRulesService { get; set; } = null!;
        
        GameInformation? gameInfo = null;
        
        protected override async Task OnInitializedAsync()
        {
            #pragma warning disable CS8600
            _gameManager.OnStateChanged = StateHasChanged; 
            _gameInfo = await gameRulesService.GetGameRulesAsync();
            #pragma warning restore CS8600
            //await gameManager.StartNewGame();
        }
    }
}

