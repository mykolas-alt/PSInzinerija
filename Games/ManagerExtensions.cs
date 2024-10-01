using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace PSInzinerija1.Games
{
    public static class GameManagerExtensions
    {
        public static async Task SaveStateSessionStorage(this IGameManager manager, ProtectedSessionStorage sessionStorage)
        {
            await sessionStorage.SetAsync(manager.GameID, manager.SerializedStatistics);
        }

        public static async Task LoadFromSessionStorage(this IGameManager manager, ProtectedSessionStorage sessionStorage)
        {
            var result = await sessionStorage.GetAsync<string>(manager.GameID);
            if (result.Success)
            {
                manager.LoadStatisticsFromJSON(result.Value);
            }
        }
    }
}