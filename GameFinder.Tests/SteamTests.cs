using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.InteropServices;
using GameFinder.StoreHandlers.Steam;
using Xunit;

namespace GameFinder.Tests;

public class SteamTests
{
    private static (List<SteamGame> expectedGames, SteamHandler handler) SetupTest()
    {
        var fileSystem = new MockFileSystem();

        var steamPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                "Steam")
            : Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Steam");

        var basePath = Path.Combine(steamPath, "steamapps");
        var commonPath = Path.Combine(basePath, "common");
        
        var steamGames = new List<SteamGame>
        {
            new(431960, "Wallpaper Engine", Path.Combine(commonPath, "wallpaper_engine")),
            new(570940, "DARK SOULS™: REMASTERED", Path.Combine(commonPath, "DARK SOULS REMASTERED"))
        };

        var libraryFoldersPath = Path.Combine(basePath, "libraryfolders.vdf");

        var libraryFoldersContent = @$"
""libraryfolders""
{{
    ""0""
    {{
        ""path""        ""{steamPath.Replace("\\", "\\\\")}""
    }}
}}
";

        fileSystem.AddFile(libraryFoldersPath, new MockFileData(libraryFoldersContent));

        foreach (var steamGame in steamGames)
        {
            var manifestPath = Path.Combine(basePath, $"appmanifest_{steamGame.AppId}.acf");

            var installDir = Path.GetFileName(steamGame.Path);
            
            var manifestContent = $@"
""AppState""
{{
    ""appid""       ""{steamGame.AppId}""
    ""name""        ""{steamGame.Name}""
    ""installdir""      ""{installDir}""
}}
";
            
            fileSystem.AddFile(manifestPath, new MockFileData(manifestContent));
        }

        var handler = new SteamHandler(fileSystem, null);
        return (steamGames, handler);
    }
    
    [Fact]
    public void TestFindAllGames()
    {
        var (expectedGames, handler) = SetupTest();

        var results = handler.FindAllGames().ToList();
        
        var actualGames = results.Select(tuple =>
        {
            var (game, error) = tuple;
            Assert.True(game is not null, error);
            return game;
        }).ToList();
        
        Assert.Equal(expectedGames.Count, actualGames.Count);
        Assert.Equal(expectedGames, actualGames);
    }
}

