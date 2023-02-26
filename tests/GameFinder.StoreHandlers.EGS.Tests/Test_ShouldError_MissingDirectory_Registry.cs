using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingDirectory_Registry(MockFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);
        fs.Directory.Delete(manifestDir);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"The manifest directory {manifestDir} does not exist!");
    }
}
