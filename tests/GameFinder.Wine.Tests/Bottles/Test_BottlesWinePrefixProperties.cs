using GameFinder.Wine.Bottles;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_GetBottlesConfigurationFile(AbsolutePath prefixDirectory)
    {
        var bottleWinePrefix = new BottlesWinePrefix
        {
            ConfigurationDirectory = prefixDirectory,
        };
        bottleWinePrefix.GetBottlesConfigFile().Should().Be(prefixDirectory.CombineUnchecked("bottle.yml"));
    }
}
