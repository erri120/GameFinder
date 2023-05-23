using System.Globalization;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_GetManifestPath(InMemoryFileSystem fs, int id, string name)
    {
        var path = fs.GetKnownPath(KnownPath.TempDirectory)
            .CombineUnchecked("common")
            .CombineUnchecked(name);

        var expectedManifestPath = fs
            .GetKnownPath(KnownPath.TempDirectory)
            .CombineUnchecked($"{id.ToString(CultureInfo.InvariantCulture)}.acf");

        var game = new SteamGame(SteamGameId.From(id), name, path, CloudSavesDirectory: null);
        game.GetManifestPath().Should().Be(expectedManifestPath);
    }
}
