using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EGS.Tests.AutoData;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, EGSAutoData]
    public void Test_ShouldError_InvalidManifest_Exception(MockFileSystem fs,
        InMemoryRegistry registry, string manifestItemName)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        var randomBytes = new byte[128];
        Random.Shared.NextBytes(randomBytes);

        var manifestItem = fs.Path.Combine(manifestDir, $"{manifestItemName}.item");
        fs.AddFile(manifestItem, new MockFileData(randomBytes));

        var error = handler.ShouldOnlyBeOneError();
        error.Should().StartWith($"Unable to deserialize file {manifestItem}:\n");
    }

    [Theory, EGSAutoData]
    public void Test_ShouldError_InvalidManifest_Null(MockFileSystem fs,
        InMemoryRegistry registry, string manifestItemName)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        var manifestItem = fs.Path.Combine(manifestDir, $"{manifestItemName}.item");
        fs.AddFile(manifestItem, new MockFileData("null"));

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Unable to deserialize file {manifestItem}");
    }
}
