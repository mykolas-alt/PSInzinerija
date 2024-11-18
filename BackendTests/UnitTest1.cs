using PSInzinerija1.Services;

namespace BackendTests;

public class UnitTest1
{
    [Fact]
    public async Task GameRulesAPIService_ConstructorThrowsIfNullArgument()
    {
        var gameRulesAPIService = new GameRulesAPIService(null);

        var gameInfo = await gameRulesAPIService.GetGameRulesAsync();

        Assert.Empty(gameInfo.Rules);
    }
}