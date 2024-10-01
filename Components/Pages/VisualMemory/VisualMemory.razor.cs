using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using PSInzinerija1.Games;
using PSInzinerija1.Games.VisualMemory;

namespace PSInzinerija1.Components.Pages.VisualMemory
{
    public partial class VisualMemory
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }
        VisualMemoryManager Manager { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        protected override async Task OnInitializedAsync()
        {
            Manager = new VisualMemoryManager();
            Manager.OnStatisticsChanged += () => Manager.SaveStateSessionStorage(SessionStorage);
            await Manager.StartNewGame();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
            }
        }

        protected async Task FetchDataAsync()
        {
            await Manager.LoadFromSessionStorage(SessionStorage);
            StateHasChanged();
        }
    }
}