using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_NoManifestDir(InMemoryFileSystem fs)
    {
        var manifestDir = OriginHandler.GetManifestDir(fs);
        var handler = new OriginHandler(fs);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest folder {manifestDir.GetFullPath()} does not exist!");
    }
}
