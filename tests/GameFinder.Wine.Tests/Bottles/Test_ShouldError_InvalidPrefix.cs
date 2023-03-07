using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingBottleConfigFile(MockFileSystem fs, string bottleName)
    {
        var (prefixDirectory, prefixManager) = CreateBottle(fs, bottleName);

        fs.AddDirectory(fs.Path.Combine(prefixDirectory, "drive_c"));
        fs.AddEmptyFile(fs.Path.Combine(prefixDirectory, "system.reg"));
        fs.AddEmptyFile(fs.Path.Combine(prefixDirectory, "user.reg"));

        var bottlesConfigFile = fs.Path.Combine(prefixDirectory, "bottle.yml");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsT1)
            .Which
            .AsT1
            .Should()
            .Be(PrefixDiscoveryError.From($"Bottles configuration file is missing at {bottlesConfigFile}"));
    }
}
