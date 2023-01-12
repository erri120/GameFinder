using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EGS.Tests.AutoData;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, EGSAutoData]
    public void Test_ShouldWork_FindAllGamesById(MockFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);
        var expectedGames = SetupGames(fs, manifestDir).ToArray();

        var results = handler.FindAllGamesById(out var errors);
        errors.Should().BeEmpty();

        results.Should().ContainKeys(expectedGames.Select(game => game.CatalogItemId));
        results.Should().ContainValues(expectedGames);
    }
}
