using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EGS.Tests.AutoData;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, EGSAutoData]
    public void Test_ShouldError_InvalidManifest(MockFileSystem fs, InMemoryRegistry registry, string manifestItemName)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        var randomBytes = new byte[128];
        Random.Shared.NextBytes(randomBytes);

        var manifestItem = fs.Path.Combine(manifestDir, $"{manifestItemName}.item");
        fs.AddFile(manifestItem, new MockFileData(randomBytes));

        var results = handler.FindAllGames().ToArray();
        results.Should().SatisfyRespectively(result =>
        {
            result.Game.Should().BeNull();
            result.Error.Should().StartWith($"Unable to deserialize file {manifestItem}");
        });
    }
}
