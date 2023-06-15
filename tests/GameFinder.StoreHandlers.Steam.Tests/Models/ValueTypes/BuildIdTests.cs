using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace GameFinder.StoreHandlers.Steam.Tests.Models.ValueTypes;

public class BuildIdTests
{
    [Fact]
    public void Test_Empty() { BuildId.Empty.Value.Should().Be(0); }

    [Theory]
    [InlineData(6248449, "https://steamdb.info/patchnotes/6248449")]
    public void Test_SteamDbUpdateNotesUrl(uint input, string expectedUrl)
    {
        var buildId = BuildId.From(input);
        buildId.GetSteamDbUpdateNotesUrl().Should().Be(expectedUrl);
    }
}
