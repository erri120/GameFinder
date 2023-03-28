using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingDirectory_Registry(InMemoryFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);
        fs.DeleteDirectory(manifestDir, recursive: true);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"The manifest directory {manifestDir} does not exist!");
    }
}
