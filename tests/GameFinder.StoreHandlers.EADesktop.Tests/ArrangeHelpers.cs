using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using GameFinder.RegistryUtils;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    private static (EADesktopHandler handler, IDirectoryInfo parentFolder) SetupHandler(
        MockFileSystem fs, string id)
    {
        var dataFolder = EADesktopHandler.GetDataFolder(fs);
        var parentFolder = dataFolder.CreateSubdirectory(id);

        var handler = new EADesktopHandler(fs);
        return (handler, parentFolder);
    }

    private static IEnumerable<EADesktopGame> SetupGames(MockFileSystem fs,
        InMemoryRegistry registry, string keyName, IDirectoryInfo parentFolder)
    {
        var fixture = new Fixture();

        var baseKey = registry.AddKey(RegistryHive.LocalMachine, $"SOFTWARE\\{keyName}");

        var installInfoFile = EADesktopHandler.GetInstallInfoFile(parentFolder);

        fixture.Customize<EADesktopGame>(composer => composer
            .FromFactory<string, string>((softwareID, baseSlug) =>
            {
                var baseInstallPath = fs.Path.Combine(fs.Path.GetTempPath(), baseSlug);
                var installerDataPath = fs.Path.Combine(baseInstallPath, "__Installer", "installerdata.xml");

                fs.AddDirectory(baseInstallPath);
                fs.AddFile(installerDataPath, new MockFileData(string.Empty));

                var installCheckKey = baseKey.AddSubKey(baseSlug);
                installCheckKey.AddValue("Install Dir", baseInstallPath + "\\");

                var installCheck = $"[{installCheckKey.GetName()}\\Install Dir]__Installer\\installerdata.xml";
                var game = new EADesktopGame(softwareID, baseSlug, baseInstallPath, installCheck, installInfoFile.FullName);

                return game;
            })
            .OmitAutoProperties());

        var games = fixture.CreateMany<EADesktopGame>().ToArray();

        var installInfos = games.Select(game => new InstallInfo
        {
            BaseSlug = game.BaseSlug,
            BaseInstallPath = game.BaseInstallPath + "\\",
            InstallCheck = game.InstallCheck,
            SoftwareID = game.SoftwareID,
        }).ToList();

        var installInfo = new InstallInfoFile
        {
            InstallInfos = installInfos,
            Schema = new Schema
            {
                Version = EADesktopHandler.SupportedSchemaVersion,
            },
        };

        var fileContents = JsonSerializer.Serialize(installInfo, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });

        fs.AddFile(installInfoFile.FullName, fileContents);

        return games;
    }
}
