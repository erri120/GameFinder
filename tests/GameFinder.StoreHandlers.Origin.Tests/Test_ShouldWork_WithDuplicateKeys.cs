using System.Web;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_WithDuplicateKeys(InMemoryFileSystem fs, string manifestName,
        string id, AbsolutePath installPath)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var manifest = manifestDir.Combine($"{manifestName}.mfst");
        fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(id)}" +
                             $"&ID={HttpUtility.UrlEncode(id)}" +
                             $"&dipInstallPath={HttpUtility.UrlEncode(installPath.GetFullPath())}" +
                             $"&dipinstallpath={HttpUtility.UrlEncode(installPath.GetFullPath())}");

        var results = handler.FindAllGames().ToArray();

        var games = results.ShouldOnlyBeGames().ToArray();
        games.Should().HaveCount(1);

        var game = games[0];
        game.Id.Should().Be(OriginGameId.From(id));
        game.InstallPath.Should().Be(installPath);
    }
}
