using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    private static (EGSHandler handler, string manifestDir) SetupHandler(MockFileSystem fs, InMemoryRegistry registry)
    {
        var fixture = new Fixture();

        var manifestDirName = fixture.Create<string>();
        var manifestDir = fs.Path.Combine(fs.Path.GetTempPath(), manifestDirName);

        fs.AddDirectory(manifestDir);

        var regKey = registry.AddKey(RegistryHive.CurrentUser, EGSHandler.RegKey);
        regKey.AddValue(EGSHandler.ModSdkMetadataDir, manifestDir);

        var handler = new EGSHandler(registry, fs);
        return (handler, manifestDir);
    }

    private static IEnumerable<EGSGame> SetupGames(MockFileSystem fs, string manifestDir)
    {
        var fixture = new Fixture();

        fixture
            .Customize<EGSGame>(composer => composer
                .FromFactory<string, string>((catalogItemId, displayName) =>
                {
                    var manifestItem = fs.Path.Combine(manifestDir, $"{catalogItemId}.item");
                    var installLocation = fs.Path.Combine(manifestDir, displayName);

                    var mockData = $@"{{
    ""CatalogItemId"": ""{catalogItemId}"",
    ""DisplayName"": ""{displayName}"",
    ""InstallLocation"": ""{installLocation.ToEscapedString()}""
}}";

                    fs.AddDirectory(installLocation);
                    fs.AddFile(manifestItem, mockData);

                    return new EGSGame(catalogItemId, displayName, installLocation);
                })
                .OmitAutoProperties());

        return fixture.CreateMany<EGSGame>();
    }
}
