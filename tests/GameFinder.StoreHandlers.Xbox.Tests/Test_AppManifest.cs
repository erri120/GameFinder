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

        var result = XboxHandler.ParseAppManifest(fs, appManifestPath);
        result.IsT0.Should().BeTrue();
        result.IsT1.Should().BeFalse();

        var game = result.AsT0;
        game.Id.Should().Be(XboxGameId.From(id));
        game.DisplayName.Should().Be(displayName);
        game.Path.Should().Be(appManifestPath.Parent);
    }
}
