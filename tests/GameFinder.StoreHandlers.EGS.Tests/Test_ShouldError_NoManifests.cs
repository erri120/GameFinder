using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoData]
    public void Test_ShouldError_NoManifests(MockFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        var results = handler.FindAllGames().ToArray();
        var error = results.ShouldOnlyBeOneError();

        error.Should().Be($"The manifest directory {manifestDir} does not contain any .item files");
    }
}
