using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using PSInzinerija1.Games;

namespace PSInzinerija1.Extensions
{
    public static class ProtectedSessionStorageExtensions
    {
        public static async Task SaveStateSessionStorage(this ProtectedSessionStorage sessionStorage, IGameManager manager)
        {
            await sessionStorage.SetAsync(manager.GameID.ToString(), manager.SerializedStatistics);
        }

        public static async Task LoadFromSessionStorage(this ProtectedSessionStorage sessionStorage, IGameManager manager)
        {
            var result = await sessionStorage.GetAsync<string>(manager.GameID.ToString());
            if (result.Success)
            {
                manager.LoadStatisticsFromJSON(result.Value);
            }
        }
    }
}