using System.IO.Abstractions.TestingHelpers;
using System.Web;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingId(MockFileSystem fs, string manifestName)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var manifest = fs.Path.Combine(manifestDir, $"{manifestName}.mfst");
        fs.AddFile(manifest, "");

        var results = handler.FindAllGames().ToArray();
        var error = results.ShouldOnlyBeOneError();

        error.Should().Be($"Manifest {manifest} does not have a value \"id\"");
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingInstallPath(MockFileSystem fs,
        string manifestName, string id)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var manifest = fs.Path.Combine(manifestDir, $"{manifestName}.mfst");
        fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(id)}");

        var results = handler.FindAllGames().ToArray();
        var error = results.ShouldOnlyBeOneError();

        error.Should().Be($"Manifest {manifest} does not have a value \"dipInstallPath\"");
    }
}
