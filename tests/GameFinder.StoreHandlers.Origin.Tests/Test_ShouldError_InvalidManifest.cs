using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_InvalidManifest(InMemoryFileSystem fs, string manifestName)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var randomBytes = new byte[128];
        Random.Shared.NextBytes(randomBytes);

        var manifest = manifestDir.Combine($"{manifestName}.mfst");
        fs.AddFile(manifest, randomBytes);

        // can't seem to reach the exception, even random garbage doesn't throw
        // an exception...
        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have a value \"id\"");
    }
}
