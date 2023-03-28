using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_SteamNotFound_DefaultPaths(InMemoryFileSystem fs)
    {
        var handler = new SteamHandler(fs, registry: null);

        var defaultSteamDirectories = SteamHandler
            .GetDefaultSteamDirectories(fs)
            .ToArray();

        foreach (var defaultSteamDirectory in defaultSteamDirectories)
        {
            var error = handler.ShouldOnlyBeOneError();
            error.Should().Be("Unable to find Steam in one of the default paths");

            fs.AddDirectory(defaultSteamDirectory);
        }
    }
}
