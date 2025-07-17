using System.Globalization;
using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingGameName(IFileSystem fileSystem, InMemoryRegistry registry, string keyName, long gameId)
    {
        var (handler, gogKey) = SetupHandler(fileSystem, registry);

        var invalidKey = gogKey.AddSubKey(keyName);
        invalidKey.AddValue("gameId", gameId.ToString(CultureInfo.InvariantCulture));

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"{invalidKey.GetName()} doesn't have a string value \"gameName\"");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingPath(IFileSystem fileSystem, InMemoryRegistry registry, string keyName, long gameId, string gameName)
    {
        var (handler, gogKey) = SetupHandler(fileSystem, registry);

        var invalidKey = gogKey.AddSubKey(keyName);
        invalidKey.AddValue("gameId", gameId.ToString(CultureInfo.InvariantCulture));
        invalidKey.AddValue("gameName", gameName);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"{invalidKey.GetName()} doesn't have a string value \"path\"");
    }
}
