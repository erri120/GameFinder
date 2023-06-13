using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGames(InMemoryFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, gameKey, hardwareInfoProvider, dataFolder) = SetupHandler(fs, registry);
        var expectedGames = SetupGames(fs, gameKey, hardwareInfoProvider, dataFolder);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGamesById(InMemoryFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, gameKey, hardwareInfoProvider, dataFolder) = SetupHandler(fs, registry);
        var expectedGames = SetupGames(fs, gameKey, hardwareInfoProvider, dataFolder).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.EADesktopGameId);
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllInterfacesGames(InMemoryFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, gameKey, hardwareInfoProvider, dataFolder) = SetupHandler(fs, registry);
        var expectedGames = SetupGames(fs, gameKey, hardwareInfoProvider, dataFolder).ToArray();
        handler.ShouldFindAllInterfacesGames(expectedGames);
    }
}
