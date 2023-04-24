using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using AutoFixture.AutoMoq;
using GameFinder.StoreHandlers.EADesktop.Crypto;
using NexusMods.Paths;

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
        AbsolutePath parentFolder)
        SetupHandler(InMemoryFileSystem fs)
    {
        var dataFolder = EADesktopHandler.GetDataFolder(fs);
        fs.AddDirectory(dataFolder);

        var hardwareInfoProvider = SetupHardwareInfoProvider();
        var handler = new EADesktopHandler(fs, hardwareInfoProvider);
        return (handler, hardwareInfoProvider, dataFolder);
    }

    [SuppressMessage("Design", "MA0051:Method is too long")]
    private static IEnumerable<EADesktopGame> SetupGames(
        InMemoryFileSystem fs, IHardwareInfoProvider hardwareInfoProvider, AbsolutePath dataFolder)
    {
        var fixture = new Fixture();

        var installInfoFile = EADesktopHandler.GetInstallInfoFile(dataFolder);
        fs.AddDirectory(installInfoFile.Parent);

        fixture.Customize<EADesktopGame>(composer => composer
            .FromFactory<string, string>((softwareId, baseSlug) =>
            {
                var baseInstallPath = fs
                    .GetKnownPath(KnownPath.TempDirectory)
                    .CombineUnchecked(baseSlug);

                var installerDataPath = baseInstallPath
                    .CombineUnchecked("__Installer")
                    .CombineUnchecked("installerdata.xml");

                fs.AddDirectory(baseInstallPath);
                fs.AddFile(installerDataPath, "");

                var game = new EADesktopGame(EADesktopGameId.From(softwareId), baseSlug, baseInstallPath);
                return game;
            })
            .OmitAutoProperties());

        var games = fixture.CreateMany<EADesktopGame>().ToArray();

        var installInfos = games.Select(game => new InstallInfo(
            game.BaseInstallPath + "\\",
            game.BaseSlug,
            InstallCheck: null,
            game.EADesktopGameId.Value))
            .ToList();

        var installInfo = new InstallInfoFile(
            installInfos,
            new Schema(EADesktopHandler.SupportedSchemaVersion));

        var encryptionKey = Decryption.CreateDecryptionKey(hardwareInfoProvider);

        using (var aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.IV = Decryption.CreateDecryptionIV();

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var stream = new MemoryStream();
            stream.Write(stackalloc byte[64]);

            using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                JsonSerializer.Serialize(cryptoStream, installInfo, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
            }

            var buffer = stream.ToArray();
            fs.AddFile(installInfoFile, buffer);
        }

        return games;
    }
}
