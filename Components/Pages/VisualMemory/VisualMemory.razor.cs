using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using PSInzinerija1.Enums;
using PSInzinerija1.Extensions;
using PSInzinerija1.Games;
using PSInzinerija1.Games.VisualMemory;
using PSInzinerija1.Models;
using PSInzinerija1.Services;

namespace PSInzinerija1.Components.Pages.VisualMemory
{
    public partial class VisualMemory
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [Inject]
        IHighScoreAPIService HighScoreAPIService { get; set; }
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }
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

            await Manager.StartNewGame();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
            }
        }

        // TODO: iskelti kitur
        private async Task SaveToDB(IGameManager gameManager)
        {
            var item = new HighScoresEntry()
            {
                Id = 1, // pakeisti i naudotojo id
                GameId = AvailableGames.VisualMemory,
                HighScore = Manager.HighScore,
            };
            var res = await HighScoreAPIService.SaveHighScoreToDbAsync(item);

            Console.WriteLine(res ? "Saved to database." : "Failed to save to database.");
        }

        private async Task FetchDataAsync()
        {
            // TODO: pakeisti i vartotojo id
            var res = await HighScoreAPIService.GetHighScoreAsync(AvailableGames.VisualMemory, 1);
            if (res != null)
            {
                Manager.SetHighScore(res.Value);
                Console.WriteLine("Loaded from database.");
            }
            else
            {
                await SessionStorage.LoadFromSessionStorage(Manager);
                Console.WriteLine("Loaded from session storage.");
            }
            StateHasChanged();
        }
    }
}