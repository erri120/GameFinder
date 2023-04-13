
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_ValidPrefix(InMemoryFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupValidWinePrefix(fs, DefaultWinePrefixManager
            .GetDefaultWinePrefixLocations(fs)
            .First());
        prefixManager
            .FindPrefixes()
            .Should()
            .ContainSingle(result => result.IsPrefix())
            .Which
            .AsPrefix()
            .ConfigurationDirectory
            .Should()
            .Be(prefixDirectory);
    }
}
