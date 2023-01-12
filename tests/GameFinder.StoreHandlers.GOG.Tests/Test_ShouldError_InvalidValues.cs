using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoData]
    public void Test_ShouldError_InvalidGameId(InMemoryRegistry registry, string keyName, string gameId)
    {
        var (handler, gogKey) = SetupHandler(registry);

        var invalidKey = gogKey.AddSubKey(keyName);
        invalidKey.AddValue("gameId", gameId);

        var results = handler.FindAllGames().ToArray();
        var error = results.ShouldOnlyBeOneError();

        error.Should().Be($"The value \"gameID\" of {invalidKey.GetName()} is not a number: \"{gameId}\"");
    }
}
