using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Web;
using GameFinder.StoreHandlers.Origin;
using Xunit;

namespace GameFinder.Tests;

public class OriginTests
{
    private static (List<OriginGame> expectedGames, OriginHandler handler) SetupTest()
    {
        var fileSystem = new MockFileSystem();
        
        var manifestDirPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Origin",
            "LocalContent"
        );

        var games = new Dictionary<string, OriginGame>
        {
            {"Origin.OFR.50.0001456.mfst", new OriginGame("Origin.OFR.50.0001456", "C:\\Games\\Titanfall2")},
            {"Origin.OFR.50.0003803.mfst", new OriginGame("Origin.OFR.50.0003803", "C:\\Games\\Slay the Spire")}
        };

        foreach (var (fileName, game) in games)
        {
            var manifestPath = Path.Combine(manifestDirPath, fileName);

            var manifestContents = $"?id={HttpUtility.UrlEncode(game.Id)}&ID={HttpUtility.UrlEncode(game.Id)}&dipInstallPath={HttpUtility.UrlEncode(game.InstallPath)}&dipinstallpath={HttpUtility.UrlEncode(game.InstallPath)}";

            fileSystem.AddFile(manifestPath, manifestContents);
        }
        
        var handler = new OriginHandler(fileSystem);

        var expectedGames = games.Select(kv => kv.Value).ToList();

        return (expectedGames, handler);
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
