using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    [Theory, AutoData]
    public void Test_ShouldError_NoSubKeys(InMemoryRegistry registry)
    {
        var (handler, gogKey) = SetupHandler(registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Registry key {gogKey.GetName()} has no sub-keys");
    }
}
