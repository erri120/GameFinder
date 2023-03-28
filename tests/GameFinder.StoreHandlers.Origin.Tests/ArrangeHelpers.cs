using System.Web;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    private static (OriginHandler handler, AbsolutePath manifestDir) SetupHandler(InMemoryFileSystem fs)
    {
        var manifestDir = OriginHandler.GetManifestDir(fs);
        fs.AddDirectory(manifestDir);

        var handler = new OriginHandler(fs);
        return (handler, manifestDir);
    }

    private static IEnumerable<OriginGame> SetupGames(InMemoryFileSystem fs, AbsolutePath manifestDir)
    {
        var fixture = new Fixture();

        fixture.Customize<OriginGame>(composer => composer
            .FromFactory<string>(id =>
            {
                var installPath = manifestDir.CombineUnchecked(id);
                var manifest = manifestDir.CombineUnchecked($"{id}.mfst");

                fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(id)}&dipInstallPath={HttpUtility.UrlEncode(installPath.GetFullPath())}");
                return new OriginGame(id, installPath);
            })
            .OmitAutoProperties());

        return fixture.CreateMany<OriginGame>();
    }
}
