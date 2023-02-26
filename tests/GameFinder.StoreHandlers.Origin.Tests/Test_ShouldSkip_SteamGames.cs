using System.IO.Abstractions.TestingHelpers;
using System.Web;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoData]
    public void Test_ShouldSkip_SteamGames(MockFileSystem fs, string manifestName,
        string id, string installPath)
    {
        var (handler, manifestDir) = SetupHandler(fs);

        var manifest = fs.Path.Combine(manifestDir, $"{manifestName}.mfst");
        fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(id)}@steam" +
                             $"&dipInstallPath={HttpUtility.UrlEncode(installPath)}");

        var results = handler.FindAllGames().ToArray();
        results.Should().BeEmpty();
    }
}
