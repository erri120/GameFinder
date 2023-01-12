using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingDirectory_DefaultPath(MockFileSystem fs, InMemoryRegistry registry)
    {
        var handler = new EGSHandler(registry, fs);

        var results = handler.FindAllGames().ToArray();
        results.Should().SatisfyRespectively(result =>
        {
            result.Game.Should().BeNull();
            result.Error.Should().Be($"The manifest directory {EGSHandler.GetDefaultManifestsPath(fs)} does not exist!");
        });
    }
}
