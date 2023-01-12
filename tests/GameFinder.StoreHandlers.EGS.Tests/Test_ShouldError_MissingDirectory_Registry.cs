using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingDirectory_Registry(MockFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        fs.Directory.Delete(manifestDir);

        var results = handler.FindAllGames().ToArray();
        results.Should().SatisfyRespectively(result =>
        {
            result.Game.Should().BeNull();
            result.Error.Should().Be($"The manifest directory {manifestDir} does not exist!");
        });
    }
}
