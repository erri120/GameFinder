using System.Web;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingId(InMemoryFileSystem fs, string manifestName)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var manifest = manifestDir.Combine($"{manifestName}.mfst");
        fs.AddFile(manifest, "");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have a value \"id\"");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingInstallPath(InMemoryFileSystem fs,
        string manifestName, string id)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var manifest = manifestDir.Combine($"{manifestName}.mfst");
        fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(id)}");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have a value \"dipInstallPath\"");
    }
}
