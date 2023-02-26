using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGames(InMemoryRegistry registry)
    {
        var (handler, gogKey) = SetupHandler(registry);
        var expectedGames = SetupGames(gogKey);

        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoData]
    public void Test_ShouldWork_FindAllGamesById(InMemoryRegistry registry)
    {
        var (handler, gogKey) = SetupHandler(registry);
        var expectedGames = SetupGames(gogKey).ToArray();

        handler.ShouldFindAllGamesById(expectedGames, game => game.Id);
    }
}
