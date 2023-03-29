using System.Text;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public partial class XboxTests
{
    private static byte[] CreateGamingRootFile(ICollection<AbsolutePath> folders)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.Unicode);
        writer.Write(0x58424752);
        writer.Write((uint)folders.Count);

        foreach (var folder in folders)
        {
            var nameBytes = Encoding.Unicode.GetBytes(folder.FileName);
            writer.Write(nameBytes);
            writer.Write('\0');
        }

        var bytes = ms.ToArray();
        return bytes;
    }

    private static string CreateAppManifestFile(string id, string displayName)
    {
        var xmlContents = $"""
<?xml version="1.0" encoding="UTF-8"?>
<Package xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10" xmlns:desktop6="http://schemas.microsoft.com/appx/manifest/desktop/windows10/6" xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10" xmlns:uap3="http://schemas.microsoft.com/appx/manifest/uap/windows10/3" xmlns:wincap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/windowscapabilities" xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities" IgnorableNamespaces="uap uap3 desktop desktop6 wincap rescap" xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10">
  <Identity Name="{id}" />
  <Properties>
    <DisplayName>{displayName}</DisplayName>
  </Properties>
</Package>
""";

        return xmlContents;
    }

    private static XboxHandler SetupHandler(InMemoryFileSystem fs, AbsolutePath appFolder)
    {
        fs.AddDirectory(fs.FromFullPath("/"));

        var gamingRootFileContents = CreateGamingRootFile(new[] { appFolder });
        var gamingRootFilePath = fs
            .EnumerateRootDirectories()
            .First()
            .CombineUnchecked(".GamingRoot");

        fs.AddFile(gamingRootFilePath, gamingRootFileContents);
        return new XboxHandler(fs);
    }

    private static IEnumerable<XboxGame> SetupGames(InMemoryFileSystem fs, AbsolutePath appFolder)
    {
        var fixture = new Fixture();

        fixture.Customize<XboxGame>(composer => composer
            .FromFactory<string, string>((id, displayName) =>
            {
                var gamePath = appFolder.CombineUnchecked(id);
                var appManifestPath = gamePath.CombineUnchecked("appmanifest.xml");
                var appManifestContents = CreateAppManifestFile(id, displayName);

                fs.AddDirectory(gamePath);
                fs.AddFile(appManifestPath, appManifestContents);

                var game = new XboxGame(id, displayName, gamePath);
                return game;
            })
            .OmitAutoProperties());

        return fixture.CreateMany<XboxGame>();
    }
}
