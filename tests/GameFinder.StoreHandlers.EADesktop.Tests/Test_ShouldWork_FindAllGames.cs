using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGames(MockFileSystem fs, string id,
        InMemoryRegistry registry, string keyName)
    {
        var (handler, parentFolder) = SetupHandler(fs, id);
        var expectedGames = SetupGames(fs, registry, keyName, parentFolder);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGamesById(MockFileSystem fs, string id,
        InMemoryRegistry registry, string keyName)
    {
        var (handler, parentFolder) = SetupHandler(fs, id);
        var expectedGames = SetupGames(fs, registry, keyName, parentFolder).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.SoftwareID);
    }
}
