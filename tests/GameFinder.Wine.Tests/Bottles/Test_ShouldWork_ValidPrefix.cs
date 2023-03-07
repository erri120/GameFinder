using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_ValidPrefix(MockFileSystem fs, string bottleName)
    {
        var (prefixDirectory, prefixManager) = SetupValidBottlesPrefix(fs, bottleName);

        prefixManager
            .FindPrefixes()
            .Should()
            .ContainSingle(result => result.IsT0)
            .Which
            .AsT0
            .ConfigurationDirectory
            .Should()
            .Be(prefixDirectory);
    }
}
