using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using Shared.Enums;
using Frontend.Services;
using Frontend.Games.VisualMemory;
using Frontend.Games;
using Frontend.Extensions;
using PSInzinerija1.Shared.Data.Models;
using PSInzinerija1.Shared.Data.Models.Stats;

namespace Frontend.Components.Pages.VisualMemory
{
    public partial class VisualMemory : ComponentBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [Inject]
        HighScoreAPIService HighScoreAPIService { get; set; }
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }
        [Inject]
        StatsAPIService<VisualMemoryStats> StatsAPIService { get; set; }
        [Inject]
        ILogger<VisualMemory> Logger { get; set; }
        VisualMemoryManager Manager { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        protected override async Task OnInitializedAsync()
        {
            Manager = new VisualMemoryManager();
            Manager.OnStatisticsChanged += async () =>
            {
                await SaveToDB(Manager);
                await SessionStorage.SaveStateSessionStorage(Manager);
            };
            Manager.OnScoreChanged += async () =>
            {
                await SaveStatsToDB(Manager);
                StateHasChanged();
            };

            await Manager.StartNewGame();
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
        private async Task SaveToDB(IGameManager gameManager)
        {
            var highScore = Manager.HighScore;
            var res = await HighScoreAPIService.SaveHighScoreToDbAsync(gameManager.GameID, highScore);

            Logger.LogInformation(res ? "Saved to database." : "Failed to save to database.");
        }

        private async Task DeleteHS(IGameManager gameManager)
        {
            var res = await HighScoreAPIService.DeleteFromDbAsync(gameManager.GameID);

            Logger.LogInformation(res ? "Deleted successfully" : "Failed to delete");
        }

        private async Task FetchDataAsync()
        {
            var res = await HighScoreAPIService.GetHighScoreAsync(AvailableGames.VisualMemory);
            if (res != null)
            {
                Manager.SetHighScore(res.Value);
                Logger.LogInformation("Loaded from database.");
            }
            else
            {
                await SessionStorage.LoadFromSessionStorage(Manager);
                Logger.LogInformation("Loaded from session storage.");
            }
            StateHasChanged();
        }

        private async Task SaveStatsToDB(VisualMemoryManager gameManager)
        {
            var stats = new VisualMemoryStats
            {
                RecentScore = gameManager.RecentScore,
                GameMistakes = gameManager.GameMistakes
            };
            await StatsAPIService.SaveStatsAsync(gameManager.GameID, stats);
        }

        private async Task FetchStatsAsync()
        {
            var stats = await StatsAPIService.GetStatsAsync(AvailableGames.VisualMemory);
            if (stats != null)
            {
                Manager.RecentScore = stats.RecentScore;
                Manager.GameMistakes = stats.GameMistakes;
                Logger.LogInformation("Loaded from database.");
            }
            StateHasChanged();
        }
    }
}