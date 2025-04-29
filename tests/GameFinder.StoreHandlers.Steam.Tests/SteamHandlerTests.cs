using FluentResults.Extensions.FluentAssertions;
using GameFinder.Common;
using GameFinder.StoreHandlers.Steam.Services;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Steam.Tests;

public class SteamHandlerTests
{
    [Fact]
    public void Test_FindAllGames()
    {
        var fs = new InMemoryFileSystem();

        var steamHandler = new SteamHandler(fs, registry: null);
        steamHandler.FindAllGames().Should().BeEmpty();

        var steamPath = SteamLocationFinder.GetDefaultSteamInstallationPaths(fs).First();
        fs.AddDirectory(steamPath);

        var libraryFoldersFilePath = SteamLocationFinder.GetLibraryFoldersFilePath(steamPath);
        fs.AddEmptyFile(libraryFoldersFilePath);

        steamHandler.FindAllGames().Should().BeEmpty();

        var libraryFoldersManifest = ArrangeHelper.CreateLibraryFoldersManifest(libraryFoldersFilePath);
        LibraryFoldersManifestWriter.Write(libraryFoldersManifest, libraryFoldersFilePath).Should().BeSuccess();

        var expectedSteamGames = new List<SteamGame>();
        foreach (var libraryFolder in libraryFoldersManifest)
        {
            fs.AddDirectory(libraryFolder.Path);
            var appManifestPath = ArrangeHelper.CreateAppManifestPath(fs, libraryFolder.Path);
            var appManifest = ArrangeHelper.CreateAppManifest(appManifestPath);
            AppManifestWriter.Write(appManifest, appManifestPath).Should().BeSuccess();

            var steamGame = new SteamGame
            {
                AppManifest = appManifest,
                LibraryFolder = libraryFolder,
                SteamPath = steamPath,
            };

            expectedSteamGames.Add(steamGame);
        }

        var results = steamHandler.FindAllGames().ToArray();
        results
            .Select(x => x.Value)
            .Should().AllBeOfType<SteamGame>()
            .Which
            .Should().Equal(expectedSteamGames);
    }
}
