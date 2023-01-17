using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography;
using System.Text.Json;
using AutoFixture.AutoMoq;
using GameFinder.StoreHandlers.EADesktop.Crypto;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    private static IHardwareInfoProvider SetupHardwareInfoProvider()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        return fixture.Create<IHardwareInfoProvider>();
    }

    private static (
        EADesktopHandler handler,
        IHardwareInfoProvider hardwareInfoProvider,
        IDirectoryInfo parentFolder)
        SetupHandler(MockFileSystem fs)
    {
        var dataFolder = EADesktopHandler.GetDataFolder(fs);
        fs.AddDirectory(dataFolder.FullName);

        var hardwareInfoProvider = SetupHardwareInfoProvider();
        var handler = new EADesktopHandler(fs, hardwareInfoProvider);
        return (handler, hardwareInfoProvider, dataFolder);
    }

    [SuppressMessage("Design", "MA0051:Method is too long")]
    private static IEnumerable<EADesktopGame> SetupGames(
        MockFileSystem fs, IHardwareInfoProvider hardwareInfoProvider, IDirectoryInfo dataFolder)
    {
        var fixture = new Fixture();

        var installInfoFile = EADesktopHandler.GetInstallInfoFile(dataFolder);
        installInfoFile.Directory!.Create();

        fixture.Customize<EADesktopGame>(composer => composer
            .FromFactory<string, string>((softwareID, baseSlug) =>
            {
                var baseInstallPath = fs.Path.Combine(fs.Path.GetTempPath(), baseSlug);
                var installerDataPath = fs.Path.Combine(baseInstallPath, "__Installer", "installerdata.xml");

                fs.AddDirectory(baseInstallPath);
                fs.AddFile(installerDataPath, new MockFileData(string.Empty));

                var game = new EADesktopGame(softwareID, baseSlug, baseInstallPath);

                return game;
            })
            .OmitAutoProperties());

        var games = fixture.CreateMany<EADesktopGame>().ToArray();

        var installInfos = games.Select(game => new InstallInfo
        {
            BaseSlug = game.BaseSlug,
            BaseInstallPath = game.BaseInstallPath + "\\",
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

        var encryptionKey = Decryption.CreateDecryptionKey(hardwareInfoProvider);

        using (var aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.IV = Decryption.CreateDecryptionIV();

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var fileStream = installInfoFile.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Write);
            fileStream.Write(new byte[64]);

            using var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write);
            JsonSerializer.Serialize(cryptoStream, installInfo, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }

        return games;
    }
}
