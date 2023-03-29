using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public partial class XboxTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGames(InMemoryFileSystem fs, AbsolutePath appFolder)
    {
        var handler = SetupHandler(fs, appFolder);
        var expectedGames = SetupGames(fs, appFolder);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGamesById(InMemoryFileSystem fs, AbsolutePath appFolder)
    {
        var handler = SetupHandler(fs, appFolder);
        var expectedGames = SetupGames(fs, appFolder).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.Id);
    }
}
