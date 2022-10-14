using System.Collections.Generic;
using System.Linq;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.GOG;
using Xunit;

namespace GameFinder.Tests;

public class GOGTests
{
    private static (IRegistry registry, List<GOGGame> expectedGames) SetupTest()
    {
        var registry = new InMemoryRegistry();

        var expectedGames = new List<GOGGame>
        {
            new(1971477531, "Gwent", "C:\\Games\\Gwent"),
            new(1207666073, "Akalabeth: World of Doom", "C:\\Games\\Akalabeth")
        };

        foreach (var expectedGame in expectedGames)
        {
            var key = registry.AddKey(RegistryHive.LocalMachine, $"SOFTWARE\\GOG.com\\Games\\{expectedGame.Id}");
            key.AddValue("gameID", $"{expectedGame.Id}");
            key.AddValue("gameName", expectedGame.Name);
            key.AddValue("path", expectedGame.Path);
        }

        return (registry, expectedGames);
    }
    
    [Fact]
    public void TestFindAllGames()
    {
        var (registry, expectedGames) = SetupTest();

        var results = GOGHandler.FindAllGames(registry).ToList();
        
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
