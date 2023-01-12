﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;
using Xunit;

namespace GameFinder.Tests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
public class SteamTests
{
    private static (SteamHandler, SteamGame[]) SetupHandler()
    {
        var fs = new MockFileSystem();

        var defaultSteamDir = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(defaultSteamDir);

        fs.AddFile(libraryFoldersFile.FullName, $@"
""libraryfolders""
{{
    ""0""
    {{
        ""path""        ""{defaultSteamDir.FullName.Replace("\\", "\\\\", StringComparison.OrdinalIgnoreCase)}""
    }}
}}");

        var basePath = fs.Path.Combine(defaultSteamDir.FullName, "steamapps");
        var commonPath = fs.Path.Combine(basePath, "common");

        var expectedGames = new SteamGame[]
        {
            new(431960, "Wallpaper Engine", Path.Combine(commonPath, "wallpaper_engine")),
            new(570940, "DARK SOULS™: REMASTERED", Path.Combine(commonPath, "DARK SOULS REMASTERED"))
        };

        foreach (var game in expectedGames)
        {
            var manifestPath = fs.Path.Combine(basePath, $"{game.AppId}.acf");
            var manifestContents = @$"
""AppState""
{{
    ""appid""       ""{game.AppId}""
    ""name""        ""{game.Name}""
    ""installdir""      ""{game.Path.Replace("\\", "\\\\", StringComparison.OrdinalIgnoreCase)}""
}}";

            fs.AddFile(manifestPath, manifestContents);
        }

        var handler = new SteamHandler(fs, new InMemoryRegistry());

        return (handler, expectedGames);
    }

    [Fact]
    public void Test_ShouldWork_FindAllGames()
    {
        var (handler, expectedGames) = SetupHandler();

        var results = handler.FindAllGames().ToArray();
        var actualGames = results.Select(tuple =>
        {
            var (game, error) = tuple;
            Assert.True(game is not null, error);
            return game;
        }).ToArray();

        Assert.Equal(expectedGames.Length, actualGames.Length);
        Assert.Equal(expectedGames, actualGames);
    }

    [Fact]
    public void Test_ShouldWork_FindAllGamesById()
    {
        var (handler, expectedGames) = SetupHandler();

        var games = handler.FindAllGamesById(out var errors);
        Assert.Empty(errors);
        Assert.All(expectedGames, game => Assert.True(games.ContainsKey(game.AppId)));
        Assert.Equal(expectedGames, games.Values);
    }

    [Fact]
    public void Test_ShouldError_NoSteam_DefaultPath()
    {
        var fs = new MockFileSystem();
        var defaultSteamDirString = $"[{SteamHandler.GetDefaultSteamDirectories(fs).Select(dir => dir.FullName).Aggregate((a, b) => $"{a}, {b}")}]";

        var handler = new SteamHandler(fs, null);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Unable to find Steam in one of the default paths {defaultSteamDirString}", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_NoSteam_Registry()
    {
        var fs = new MockFileSystem();
        var defaultSteamDirString = $"[{SteamHandler.GetDefaultSteamDirectories(fs).Select(dir => dir.FullName).Aggregate((a, b) => $"{a}, {b}")}]";

        var handler = new SteamHandler(fs, new InMemoryRegistry());

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Unable to find Steam in the registry and one of the default paths {defaultSteamDirString}", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_NoSteam_MissingDirectoryFromRegistry()
    {
        var fs = new MockFileSystem();

        var steamDir = fs.DirectoryInfo.New(fs.Path.Combine(fs.Path.GetTempPath(), "Steam"));
        var defaultSteamDirString = $"[{SteamHandler.GetDefaultSteamDirectories(fs).Select(dir => dir.FullName).Aggregate((a, b) => $"{a}, {b}")}]";

        var registry = new InMemoryRegistry();
        var key = registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        key.AddValue("SteamPath", steamDir.FullName);

        var handler = new SteamHandler(fs, registry);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Unable to find Steam in one of the default paths {defaultSteamDirString} and the path from the registry does not exist: {steamDir.FullName}", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_NoSteam_MissingLibraryFoldersFileFromRegistry()
    {
        var fs = new MockFileSystem();

        var steamDir = fs.DirectoryInfo.New(fs.Path.Combine(fs.Path.GetTempPath(), "Steam"));
        var defaultSteamDirString = $"[{SteamHandler.GetDefaultSteamDirectories(fs).Select(dir => dir.FullName).Aggregate((a, b) => $"{a}, {b}")}]";

        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(steamDir);

        fs.AddDirectory(steamDir.FullName);

        var registry = new InMemoryRegistry();
        var key = registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        key.AddValue("SteamPath", steamDir.FullName);

        var handler = new SteamHandler(fs, registry);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Unable to find Steam in one of the default paths {defaultSteamDirString} and the path from the registry is not a valid Steam installation because {libraryFoldersFile.FullName} does not exist", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_NoManifests()
    {
        var fs = new MockFileSystem();

        var defaultSteamDir = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(defaultSteamDir);

        fs.AddFile(libraryFoldersFile.FullName, $@"
""libraryfolders""
{{
    ""0""
    {{
        ""path""        ""{defaultSteamDir.FullName.Replace("\\", "\\\\", StringComparison.OrdinalIgnoreCase)}""
    }}
}}");

        var handler = new SteamHandler(fs, new InMemoryRegistry());

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Library folder {fs.Path.Combine(defaultSteamDir.FullName, "steamapps")} does not contain any manifests", result.Error);
        });
    }

    [Fact]
    [SuppressMessage("Design", "MA0051:Method is too long")]
    public void Test_ShouldError_MissingValues()
    {
        var fs = new MockFileSystem();

        var defaultSteamDir = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(defaultSteamDir);

        fs.AddFile(libraryFoldersFile.FullName, $@"
""libraryfolders""
{{
    ""0""
    {{
        ""path""        ""{defaultSteamDir.FullName.Replace("\\", "\\\\", StringComparison.OrdinalIgnoreCase)}""
    }}
}}");

        var basePath = fs.Path.Combine(defaultSteamDir.FullName, "steamapps");

        var manifest1 = fs.Path.Combine(basePath, "1-no-appid.acf");
        const string manifest1Contents = @"
""AppState""
{
}
";

        var manifest2 = fs.Path.Combine(basePath, "2-no-name.acf");
        const string manifest2Contents = @"
""AppState""
{
    ""appid""       ""0""
}
";

        var manifest3 = fs.Path.Combine(basePath, "3-no-installdir.acf");
        const string manifest3Contents = @"
""AppState""
{
    ""appid""       ""0""
    ""name""        ""foo""
}
";

        var manifest4 = fs.Path.Combine(basePath, "4-bad-id.acf");
        const string manifest4Contents = @"
""AppState""
{
    ""appid""       ""this is not a number""
    ""name""        ""foo""
    ""installdir""      ""bar""
}
";

        fs.AddFile(manifest1, manifest1Contents);
        fs.AddFile(manifest2, manifest2Contents);
        fs.AddFile(manifest3, manifest3Contents);
        fs.AddFile(manifest4, manifest4Contents);

        var handler = new SteamHandler(fs, new InMemoryRegistry());

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Manifest {manifest1} does not have the value \"appid\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Manifest {manifest2} does not have the value \"name\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Manifest {manifest3} does not have the value \"installdir\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.StartsWith($"Exception while parsing file {manifest4}", result.Error, StringComparison.OrdinalIgnoreCase);
        });
    }
}