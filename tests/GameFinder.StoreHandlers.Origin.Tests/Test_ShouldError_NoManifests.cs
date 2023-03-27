using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_NoManifests(InMemoryFileSystem fs)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest folder {manifestDir} does not contain any .mfst files");
    }
}
