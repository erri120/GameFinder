using System.Globalization;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingGameId(InMemoryRegistry registry, string keyName)
    {
        var (handler, gogKey) = SetupHandler(registry);

        var invalidKey = gogKey.AddSubKey(keyName);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"{invalidKey.GetName()} doesn't have a string value \"gameID\"");
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingGameName(InMemoryRegistry registry, string keyName, long gameId)
    {
        var (handler, gogKey) = SetupHandler(registry);

        var invalidKey = gogKey.AddSubKey(keyName);
        invalidKey.AddValue("gameId", gameId.ToString(CultureInfo.InvariantCulture));

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"{invalidKey.GetName()} doesn't have a string value \"gameName\"");
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingPath(InMemoryRegistry registry, string keyName, long gameId, string gameName)
    {
        var (handler, gogKey) = SetupHandler(registry);

        var invalidKey = gogKey.AddSubKey(keyName);
        invalidKey.AddValue("gameId", gameId.ToString(CultureInfo.InvariantCulture));
        invalidKey.AddValue("gameName", gameName);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"{invalidKey.GetName()} doesn't have a string value \"path\"");
    }
}
