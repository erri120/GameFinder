using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    private static (SteamHandler handler, string basePath, string commonPath) SetupHandler(MockFileSystem fs)
    {
        var defaultSteamDir = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(defaultSteamDir);

        fs.AddFile(libraryFoldersFile.FullName, $@"
""libraryfolders""
{{
    ""0""
    {{
        ""path""        ""{defaultSteamDir.FullName.ToEscapedString()}""
    }}
}}");

        var basePath = fs.Path.Combine(defaultSteamDir.FullName, "steamapps");
        var commonPath = fs.Path.Combine(basePath, "common");

        var handler = new SteamHandler(fs, registry: null);
        return (handler, basePath, commonPath);
    }

    private static IEnumerable<SteamGame> SetupGames(MockFileSystem fs, string basePath, string commonPath)
    {
        var fixture = new Fixture();

        fixture.Customize<SteamGame>(composer => composer
            .FromFactory<int, string>((appId, name) =>
            {
                var path = fs.Path.Combine(commonPath, name);

                var manifest = fs.Path.Combine(basePath, $"{appId.ToString(CultureInfo.InvariantCulture)}.acf");
                fs.AddFile(manifest, @$"
""AppState""
{{
    ""appid""       ""{appId.ToString(CultureInfo.InvariantCulture)}""
    ""name""        ""{name}""
    ""installdir""      ""{name}""
}}");

                return new SteamGame(appId, name, path);
            })
            .OmitAutoProperties());

        return fixture.CreateMany<SteamGame>();
    }
}
