using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGames(MockFileSystem fs)
    {
        var (handler, hardwareInfoProvider, dataFolder) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, hardwareInfoProvider, dataFolder);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGamesById(MockFileSystem fs)
    {
        var (handler, hardwareInfoProvider, dataFolder) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, hardwareInfoProvider, dataFolder).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.SoftwareID);
    }
}
