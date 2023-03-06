using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_ValidPrefix(MockFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupValidWinePrefix(fs, DefaultWinePrefixManager
            .GetDefaultWinePrefixLocations(fs)
            .First());
        prefixManager
            .FindPrefixes()
            .Should()
            .ContainSingle(result => result.IsT0)
            .Which
            .AsT0
            .ConfigurationDirectory
            .Should()
            .Be(prefixDirectory.FullName);
    }
}
