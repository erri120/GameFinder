using System.IO.Abstractions.TestingHelpers;
using System.Web;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    private static (OriginHandler handler, string manifestDir) SetupHandler(MockFileSystem fs)
    {
        var manifestDir = OriginHandler.GetManifestDir(fs);
        fs.AddDirectory(manifestDir.FullName);

        var handler = new OriginHandler(fs);
        return (handler, manifestDir.FullName);
    }

    private static IEnumerable<OriginGame> SetupGames(MockFileSystem fs, string manifestDir)
    {
        var fixture = new Fixture();

        fixture.Customize<OriginGame>(composer => composer
            .FromFactory<string>(id =>
            {
                var installPath = fs.Path.Combine(manifestDir, id);

                var manifest = fs.Path.Combine(manifestDir, $"{id}.mfst");
                fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(id)}&dipInstallPath={HttpUtility.UrlEncode(installPath)}");

                return new OriginGame(id, installPath);
            })
            .OmitAutoProperties());

        return fixture.CreateMany<OriginGame>();
    }
}
