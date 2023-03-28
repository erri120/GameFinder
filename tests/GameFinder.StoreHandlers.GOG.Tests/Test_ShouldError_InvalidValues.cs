using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_InvalidGameId(IFileSystem fileSystem, InMemoryRegistry registry, string keyName, string gameId)
    {
        var (handler, gogKey) = SetupHandler(fileSystem, registry);

        var invalidKey = gogKey.AddSubKey(keyName);
        invalidKey.AddValue("gameId", gameId);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"The value \"gameID\" of {invalidKey.GetName()} is not a number: \"{gameId}\"");
    }
}
