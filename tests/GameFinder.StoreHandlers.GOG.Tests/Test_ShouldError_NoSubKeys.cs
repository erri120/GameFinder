using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_NoSubKeys(IFileSystem fileSystem, InMemoryRegistry registry)
    {
        var (handler, gogKey) = SetupHandler(fileSystem, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Registry key {gogKey.GetName()} has no sub-keys");
    }
}
