using GameFinder.Common;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingBottleConfigFile(InMemoryFileSystem fs, string bottleName)
    {
        var (prefixDirectory, prefixManager) = CreateBottle(fs, bottleName);

        fs.AddDirectory(prefixDirectory.Combine("drive_c"));
        fs.AddEmptyFile(prefixDirectory.Combine("system.reg"));
        fs.AddEmptyFile(prefixDirectory.Combine("user.reg"));

        var bottlesConfigFile = prefixDirectory.Combine("bottle.yml");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsError())
            .Which
            .AsError()
            .Should()
            .Be($"Bottles configuration file is missing at {bottlesConfigFile}");
    }
}
