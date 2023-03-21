using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingDirectory_DefaultPath(InMemoryFileSystem fs, InMemoryRegistry registry)
    {
        var handler = new EGSHandler(registry, fs);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"The manifest directory {EGSHandler.GetDefaultManifestsPath(fs)} does not exist!");
    }
}
