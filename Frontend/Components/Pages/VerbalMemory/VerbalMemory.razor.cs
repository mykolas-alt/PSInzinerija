using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using PSInzinerija1.Enums;
using PSInzinerija1.Exceptions;
using PSInzinerija1.Extensions;
using PSInzinerija1.Games;
using PSInzinerija1.Games.VerbalMemory;
using Frontend.Services;

namespace Frontend.Components.Pages.VerbalMemory
{
    public partial class VerbalMemory : ComponentBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [Inject]
        HighScoreAPIService HighScoreAPIService { get; set; }
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }
        [Inject]
        WordListAPIService WordListAPIService { get; set; }

        VerbalMemoryManager Manager { get; set; } = new VerbalMemoryManager();

        bool LoadFailed { get; set; } = false;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        protected override async Task OnInitializedAsync()
        {
            List<string>? wordList = null;
            try
            {
                wordList = await WordListAPIService.GetWordsFromApiAsync("SimonSaysRules.txt");
            }
            catch (WordListLoadException ex)
            {
                Console.WriteLine($"Error loading word list: {ex.Message}");
                LoadFailed = true;
            }

            if (wordList != null && wordList.Any())
            {
                Manager.OnStatisticsChanged += async () =>
                {
                    await SaveToDB(Manager);
                    await SessionStorage.SaveStateSessionStorage(Manager);
                };
                await Manager.StartNewGame(wordList);
            }
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
            var highScore = Manager.HighScore;
            var res = await HighScoreAPIService.SaveHighScoreToDbAsync(gameManager.GameID, highScore);

            Console.WriteLine(res ? "Saved to database." : "Failed to save to database.");
        }

        private async Task DeleteHS(IGameManager gameManager)
        {
            var res = await HighScoreAPIService.DeleteFromDbAsync(gameManager.GameID);

            Console.WriteLine(res ? "Deleted successfully" : "Failed to delete");
        }

        private async Task FetchDataAsync()
        {
            var res = await HighScoreAPIService.GetHighScoreAsync(AvailableGames.VerbalMemory);
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