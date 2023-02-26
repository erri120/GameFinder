using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Origin.Tests;

public partial class OriginTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGames(MockFileSystem fs)
    {
        var (handler, manifestDir) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, manifestDir);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGamesById(MockFileSystem fs)
    {
        var (handler, manifestDir) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, manifestDir).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.Id);
    }
}
