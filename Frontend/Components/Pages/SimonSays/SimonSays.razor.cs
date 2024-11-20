using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Shared.Enums;
using Frontend.Services;
using Frontend.Games.SimonSays;
using Frontend.Games;
using Frontend.Extensions;
using System.Runtime.CompilerServices;
using PSInzinerija1.Shared.Data.Models;
using PSInzinerija1.Shared.Data.Models.Stats;
using System.Runtime.InteropServices;

namespace Frontend.Components.Pages.SimonSays
{
    public partial class SimonSays : ComponentBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [Inject]
        HighScoreAPIService HighScoreAPIService { get; set; }
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }
        [Inject]
        StatsAPIService<SimonSaysStats> StatsAPIService { get; set; }
        // [Inject]
        // GameStatsService<SimonSaysStats> GameStatsService;
        [Inject]
        ILogger<SimonSays> Logger { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        private readonly SimonSaysManager gameManager = new();
        // [Inject]
        // public GameRulesAPIService GameRulesService { get; set; } = null!;

        GameInfo? gameInfo = null;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
#pragma warning disable CS8600
            gameManager.OnStateChanged = StateHasChanged;
            gameManager.OnStatisticsChanged += async () =>
            {
                await SaveToDB(gameManager);
                await SaveStatsToDB(gameManager);
                await SessionStorage.SaveStateSessionStorage(gameManager);
            };
            gameManager.OnScoreChanged += async () =>
            {
                await SaveStatsToDB(gameManager);
                StateHasChanged();
            };

            // gameInfo = await GameRulesService.GetGameRulesAsync();
            // {var userId = "user123"; // You'll need to replace this with a real user ID retrieval method
            // stats = await GameStatsService.GetStatsAsync(userId, AvailableGames.SimonSays);
            // isLoading = false;} //pakeist i actual user id
#pragma warning restore CS8600
            //await gameManager.StartNewGame();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
                await FetchStatsAsync();
            }
        }


        // TODO: iskelti kitur
        private async Task SaveToDB(IGameManager iGameManager)
        {
            var highScore = gameManager.HighScore;
            var res = await HighScoreAPIService.SaveHighScoreToDbAsync(iGameManager.GameID, highScore); 
            Logger.LogInformation(res ? "Saved to database." : "Failed to save to database.");
        }

        private async Task DeleteHS(IGameManager gameManager)
        {
            var res = await HighScoreAPIService.DeleteFromDbAsync(gameManager.GameID);

            Logger.LogInformation(res ? "Deleted successfully" : "Failed to delete");
        }

        private async Task FetchDataAsync()
        {
            var res = await HighScoreAPIService.GetHighScoreAsync(AvailableGames.SimonSays);
            if (res != null)
            {
                gameManager.SetHighScore(res.Value);
                Logger.LogInformation("Loaded from database.");
            }
            else
            {
                await SessionStorage.LoadFromSessionStorage(gameManager);
                Logger.LogInformation("Loaded from session storage.");
            }
            StateHasChanged();
        }

        private async Task SaveStatsToDB(SimonSaysManager gameManager)
        {
            var stats = new SimonSaysStats
            {
                RecentScore = gameManager.Level,
                TimePlayed = gameManager.TimePlayed
            };
            await StatsAPIService.SaveStatsAsync(AvailableGames.SimonSays, stats);
        }

        private async Task FetchStatsAsync()
        {
            var stats = await StatsAPIService.GetStatsAsync(AvailableGames.SimonSays);
            if (stats != null)
            {
                gameManager.RecentScore = stats.RecentScore;
                gameManager.TimePlayed = stats.TimePlayed;
                Logger.LogInformation("Loaded from database.");
            }
            StateHasChanged();
        }
    }
}