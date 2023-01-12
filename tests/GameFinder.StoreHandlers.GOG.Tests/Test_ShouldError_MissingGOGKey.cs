using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingGOGKey(InMemoryRegistry registry)
    {
        var handler = new GOGHandler(registry);

        var results = handler.FindAllGames().ToArray();
        var error = results.ShouldOnlyBeOneError();

        error.Should().Be($"Unable to open HKEY_LOCAL_MACHINE\\{GOGHandler.GOGRegKey}");
    }
}
