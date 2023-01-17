using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGames(MockFileSystem fs,
        InMemoryRegistry registry, string keyName)
    {
        var (handler, hardwareInfoProvider, dataFolder) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, hardwareInfoProvider, registry, keyName, dataFolder);
        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGamesById(MockFileSystem fs,
        InMemoryRegistry registry, string keyName)
    {
        var (handler, hardwareInfoProvider, dataFolder) = SetupHandler(fs);
        var expectedGames = SetupGames(fs, hardwareInfoProvider, registry, keyName, dataFolder).ToArray();
        handler.ShouldFindAllGamesById(expectedGames, game => game.SoftwareID);
    }
}
