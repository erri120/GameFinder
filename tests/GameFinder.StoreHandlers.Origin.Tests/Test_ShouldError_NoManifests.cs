using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoData]
    public void Test_ShouldError_NoManifests(MockFileSystem fs)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest folder {manifestDir} does not contain any .mfst files");
    }
}
