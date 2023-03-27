using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGames(InMemoryFileSystem fs)
    {
        var (handler, manifestDir) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, manifestDir);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGamesById(InMemoryFileSystem fs)
    {
        var (handler, manifestDir) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, manifestDir).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.Id);
    }
}
