using System.IO.Abstractions.TestingHelpers;
using System.Web;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_WithDuplicateKeys(MockFileSystem fs, string manifestName,
        string id, string installPath)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var manifest = fs.Path.Combine(manifestDir, $"{manifestName}.mfst");
        fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(id)}" +
                             $"&ID={HttpUtility.UrlEncode(id)}" +
                             $"&dipInstallPath={HttpUtility.UrlEncode(installPath)}" +
                             $"&dipinstallpath={HttpUtility.UrlEncode(installPath)}");

        var results = handler.FindAllGames().ToArray();

        var games = results.ShouldOnlyBeGames().ToArray();
        games.Should().HaveCount(1);

        var game = games[0];
        game.Id.Should().Be(id);
        game.InstallPath.Should().Be(installPath);
    }
}
