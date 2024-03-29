using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGames(InMemoryFileSystem fs)
    {
        var (handler, hardwareInfoProvider, dataFolder) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, hardwareInfoProvider, dataFolder);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGamesById(InMemoryFileSystem fs)
    {
        var (handler, hardwareInfoProvider, dataFolder) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, hardwareInfoProvider, dataFolder).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.EADesktopGameId);
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllInterfacesGames(InMemoryFileSystem fs)
    {
        var (handler, hardwareInfoProvider, dataFolder) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, hardwareInfoProvider, dataFolder).ToArray();
        handler.ShouldFindAllInterfacesGames(expectedGames);
    }
}
