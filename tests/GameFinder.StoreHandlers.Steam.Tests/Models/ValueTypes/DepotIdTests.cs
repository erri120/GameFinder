using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace GameFinder.StoreHandlers.Steam.Tests.Models.ValueTypes;

public class DepotIdTests
{
    [Fact]
    public void Test_Empty() { DepotId.Empty.Value.Should().Be(0); }

    [Theory]
    [InlineData(262061, "https://steamdb.info/depot/262061")]
    public void Test_SteamDbUpdateNotesUrl(uint input, string expectedUrl)
    {
        var depotId = DepotId.From(input);
        depotId.SteamDbUrl.Should().Be(expectedUrl);
    }
}
