using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoData]
    public void Test_ShouldError_InvalidManifest(MockFileSystem fs, string manifestName)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var randomBytes = new byte[128];
        Random.Shared.NextBytes(randomBytes);

        var manifest = fs.Path.Combine(manifestDir, $"{manifestName}.mfst");
        fs.AddFile(manifest, new MockFileData(randomBytes));

        // can't seem to reach the exception, even random garbage doesn't throw
        // an exception...
        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have a value \"id\"");
    }
}
