using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.GOG;
using Xunit;

namespace GameFinder.Tests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
public class GOGTests
{
    private static (GOGHandler, GOGGame[]) SetupHandler()
    {
        var registry = new InMemoryRegistry();

        var expectedGames = new[]
        {
            new GOGGame(1971477531, "Gwent", "C:\\Games\\Gwent"),
            new GOGGame(1207666073, "Akalabeth: World of Doom", "C:\\Games\\Akalabeth")
        };

        foreach (var game in expectedGames)
        {
            var key = registry.AddKey(RegistryHive.LocalMachine, $"{GOGHandler.GOGRegKey}\\{game.Id}");
            key.AddValue("gameID", $"{game.Id}");
            key.AddValue("gameName", game.Name);
            key.AddValue("path", game.Path);
        }

        var handler = new GOGHandler(registry);

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
        Assert.All(expectedGames, game => Assert.True(games.ContainsKey(game.Id)));
        Assert.Equal(expectedGames, games.Values);
    }

    [Fact]
    public void Test_ShouldError_MissingGOGKey()
    {
        var registry = new InMemoryRegistry();

        var handler = new GOGHandler(registry);
        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Unable to open HKEY_LOCAL_MACHINE\\{GOGHandler.GOGRegKey}", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_NoSubKeys()
    {
        var registry = new InMemoryRegistry();
        var gogKey = registry.AddKey(RegistryHive.LocalMachine, GOGHandler.GOGRegKey);

        var handler = new GOGHandler(registry);
        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Registry key {gogKey.GetName()} has no sub-keys", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_ParseSubKey()
    {
        var registry = new InMemoryRegistry();

        var key1 = registry.AddKey(RegistryHive.LocalMachine,$"{GOGHandler.GOGRegKey}\\no-id");
        var key2 = registry.AddKey(RegistryHive.LocalMachine,$"{GOGHandler.GOGRegKey}\\id-is-not-a-number");
        var key3 = registry.AddKey(RegistryHive.LocalMachine,$"{GOGHandler.GOGRegKey}\\no-name");
        var key4 = registry.AddKey(RegistryHive.LocalMachine,$"{GOGHandler.GOGRegKey}\\no-path");

        key2.AddValue("gameID", "this is not a number");
        key3.AddValue("gameID", "0");
        key4.AddValue("gameID", "0");
        key4.AddValue("gameName", "foo");

        var handler = new GOGHandler(registry);
        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"{key1.GetName()} doesn't have a string value \"gameID\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"The value \"gameID\" of {key2.GetName()} is not a number: \"this is not a number\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"{key3.GetName()} doesn't have a string value \"gameName\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"{key4.GetName()} doesn't have a string value \"path\"", result.Error);
        });
    }
}
