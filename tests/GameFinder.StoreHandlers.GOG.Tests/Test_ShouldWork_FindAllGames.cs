using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGames(IFileSystem fileSystem, InMemoryRegistry registry)
    {
        var (handler, gogKey) = SetupHandler(fileSystem, registry);
        var expectedGames = SetupGames(fileSystem, gogKey);

        handler.ShouldFindAllGames(expectedGames);
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldWork_FindAllGamesById(IFileSystem fileSystem, InMemoryRegistry registry)
    {
        var (handler, gogKey) = SetupHandler(fileSystem, registry);
        var expectedGames = SetupGames(fileSystem, gogKey).ToArray();

        handler.ShouldFindAllGamesById(expectedGames, game => game.Id);
    }
}
