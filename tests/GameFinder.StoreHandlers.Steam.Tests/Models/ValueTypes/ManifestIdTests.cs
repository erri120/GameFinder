using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace GameFinder.StoreHandlers.Steam.Tests.Models.ValueTypes;

public class ManifestIdTests
{
    [Fact]
    public void Test_Empty() { ManifestId.DefaultValue.Value.Should().Be(0); }

    [Theory]
    [InlineData(5542773349944116172, 262061, "https://steamdb.info/depot/262061/history/?changeid=M:5542773349944116172")]
    public void Test_GetSteamDbChangesetUrl(ulong input, uint depot, string expectedUrl)
    {
        var manifestId = ManifestId.From(input);
        var depotId = DepotId.From(depot);
        manifestId.GetSteamDbChangesetUrl(depotId).Should().Be(expectedUrl);
    }
}
