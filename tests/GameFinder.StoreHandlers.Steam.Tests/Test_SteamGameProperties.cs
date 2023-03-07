using System.Globalization;
using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_GetManifestPath(MockFileSystem fs, int id, string name)
    {
        var path = fs.Path.Combine(fs.Path.GetTempPath(), "common", name);
        var expectedManifestPath = fs.Path.Combine(
            fs.Path.GetTempPath(),
            $"{id.ToString(CultureInfo.InvariantCulture)}.acf");

        var game = new SteamGame(id, name, path);

        game.GetManifestPath(fs).Should().Be(expectedManifestPath);
        game.GetManifestPath().Should().Be(expectedManifestPath);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_GetProtonPrefixDirectory_FakeFS(MockFileSystem fs, int appId, string name)
    {
        var protonPrefixDirectory = fs.Path.Combine(fs.Path.GetTempPath(), "compatdata", appId.ToString(CultureInfo.InvariantCulture));
        var path = fs.Path.Combine(fs.Path.GetTempPath(), "common", name);

        var steamGame = new SteamGame(appId, name, path);
        steamGame.GetProtonPrefixDirectory(fs.Path).Should().Be(protonPrefixDirectory);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_GetProtonPrefixDirectory_RealFS(int appId, string name)
    {
        var protonPrefixDirectory = Path.Combine(Path.GetTempPath(), "compatdata", appId.ToString(CultureInfo.InvariantCulture));
        var path = Path.Combine(Path.GetTempPath(), "common", name);

        var steamGame = new SteamGame(appId, name, path);
        steamGame.GetProtonPrefixDirectory().Should().Be(protonPrefixDirectory);
    }
}
