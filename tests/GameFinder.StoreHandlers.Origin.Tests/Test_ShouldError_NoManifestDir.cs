using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoData]
    public void Test_ShouldError_NoManifestDir(MockFileSystem fs)
    {
        var manifestDir = OriginHandler.GetManifestDir(fs);
        var handler = new OriginHandler(fs);

        var results = handler.FindAllGames().ToArray();
        var error = results.ShouldOnlyBeOneError();

        error.Should().Be($"Manifest folder {manifestDir.FullName} does not exist!");
    }
}
