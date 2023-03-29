using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public partial class XboxTests
{
    [Theory, AutoFileSystem]
    public void Test_ParseAppManifest(InMemoryFileSystem fs, AbsolutePath appManifestPath, string id, string displayName)
    {
        var xmlContents = CreateAppManifestFile(id, displayName);
        fs.AddFile(appManifestPath, xmlContents);

        var (game, error) = XboxHandler.ParseAppManifest(fs, appManifestPath);
        error.Should().BeNull();
        game.Should().NotBeNull();

        game!.Id.Should().Be(id);
        game.DisplayName.Should().Be(displayName);
        game.Path.Should().Be(appManifestPath.Parent);
    }
}
