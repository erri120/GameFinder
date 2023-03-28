using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingBottleConfigFile(InMemoryFileSystem fs, string bottleName)
    {
        var (prefixDirectory, prefixManager) = CreateBottle(fs, bottleName);

        fs.AddDirectory(prefixDirectory.CombineUnchecked("drive_c"));
        fs.AddEmptyFile(prefixDirectory.CombineUnchecked("system.reg"));
        fs.AddEmptyFile(prefixDirectory.CombineUnchecked("user.reg"));

        var bottlesConfigFile = prefixDirectory.CombineUnchecked("bottle.yml");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsT1)
            .Which
            .AsT1
            .Should()
            .Be(PrefixDiscoveryError.From($"Bottles configuration file is missing at {bottlesConfigFile}"));
    }
}
