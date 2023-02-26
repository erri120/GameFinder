using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_ShouldError_SteamNotFound_DefaultPaths(MockFileSystem fs)
    {
        var handler = new SteamHandler(fs, registry: null);

        var defaultSteamDirectories = SteamHandler
            .GetDefaultSteamDirectories(fs)
            .ToArray();

        foreach (var defaultSteamDirectory in defaultSteamDirectories)
        {
            var error = handler.ShouldOnlyBeOneError();
            error.Should().Be("Unable to find Steam in one of the default paths");

            fs.AddDirectory(defaultSteamDirectory.FullName);
        }
    }
}
