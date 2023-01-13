using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGames(MockFileSystem fs)
    {
        var (handler, basePath, commonPath) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, basePath, commonPath);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGamesById(MockFileSystem fs)
    {
        var (handler, basePath, commonPath) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, basePath, commonPath).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.AppId);
    }
}
