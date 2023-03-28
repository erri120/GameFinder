using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingGOGKey(IFileSystem fileSystem, InMemoryRegistry registry)
    {
        var handler = new GOGHandler(registry, fileSystem);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Unable to open HKEY_LOCAL_MACHINE\\{GOGHandler.GOGRegKey}");
    }
}
