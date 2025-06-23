using GameFinder.RegistryUtils;
using NexusMods.Paths;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    private static (EGSHandler handler, AbsolutePath manifestDir) SetupHandler(InMemoryFileSystem fs, InMemoryRegistry registry)
    {
        var fixture = new Fixture();

        var manifestDirName = fixture.Create<string>();
        var manifestDir = fs
            .GetKnownPath(KnownPath.TempDirectory)
            .Combine(manifestDirName);

        fs.AddDirectory(manifestDir);

        var regKey = registry.AddKey(RegistryHive.CurrentUser, EGSHandler.RegKey);
        regKey.AddValue(EGSHandler.ModSdkMetadataDir, manifestDir.GetFullPath());

        var handler = new EGSHandler(registry, fs);
        return (handler, manifestDir);
    }

    private static IEnumerable<EGSGame> SetupGames(InMemoryFileSystem fs, AbsolutePath manifestDir)
    {
        var fixture = new Fixture();

        fixture
            .Customize<EGSGame>(composer => composer
                .FromFactory<string, string>((catalogItemId, displayName) =>
                {
                    var manifestItem = manifestDir.Combine($"{catalogItemId}.item");
                    var installLocation = manifestDir.Combine(displayName);

                    var mockData = $@"{{
    ""CatalogItemId"": ""{catalogItemId}"",
    ""DisplayName"": ""{displayName}"",
    ""InstallLocation"": ""{installLocation.GetFullPath().ToEscapedString()}"",
    ""MainGameCatalogItemId"": ""{catalogItemId}"",
    ""ManifestHash"": ""{catalogItemId}_manifest""
}}";

                    fs.AddDirectory(installLocation);
                    fs.AddFile(manifestItem, mockData);

                    return new EGSGame(EGSGameId.From(catalogItemId), displayName, installLocation, new [] { catalogItemId + "_manifest" });
                })
                .OmitAutoProperties());

        return fixture.CreateMany<EGSGame>();
    }
}
