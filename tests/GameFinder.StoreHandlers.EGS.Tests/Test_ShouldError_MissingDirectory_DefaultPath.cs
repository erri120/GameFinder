using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingDirectory_DefaultPath(MockFileSystem fs, InMemoryRegistry registry)
    {
        var handler = new EGSHandler(registry, fs);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"The manifest directory {EGSHandler.GetDefaultManifestsPath(fs)} does not exist!");
    }
}
