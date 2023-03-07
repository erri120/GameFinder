using System.IO.Abstractions.TestingHelpers;
using GameFinder.Wine.Bottles;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_GetBottlesConfigurationFile(MockFileSystem fs, string prefixDirectory)
    {
        var bottleWinePrefix = new BottlesWinePrefix(prefixDirectory);

        bottleWinePrefix.GetBottlesConfigFile(fs.Path).Should().Be(fs.Path.Combine(prefixDirectory, "bottle.yml"));
        bottleWinePrefix.GetBottlesConfigFile().Should().Be(Path.Combine(prefixDirectory, "bottle.yml"));
    }
}
