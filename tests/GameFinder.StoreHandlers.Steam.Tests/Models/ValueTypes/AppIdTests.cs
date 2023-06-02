using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace GameFinder.StoreHandlers.Steam.Tests.Models.ValueTypes;

public class AppIdTests
{
    [Fact]
    public void Test_Empty() { AppId.Empty.Value.Should().Be(0); }

    [Theory]
    [InlineData(262060, "https://store.steampowered.com/app/262060")]
    public void Test_SteamStoreUrl(uint input, string expectedUrl)
    {
        var appId = AppId.From(input);
        appId.SteamStoreUrl.Should().Be(expectedUrl);
    }

    [Theory]
    [InlineData(262060, "https://steamdb.info/app/262060")]
    public void Test_SteamDbUrl(uint input, string expectedUrl)
    {
        var appId = AppId.From(input);
        appId.SteamDbUrl.Should().Be(expectedUrl);
    }

    [Theory]
    [InlineData(262060, "äüö", "https://store.steampowered.com/app/262060/?utm_source=%c3%a4%c3%bc%c3%b6")]
    public void Test_GetSteamStoreUrlWithTracking(uint input, string source, string expectedUrl)
    {
        var appId = AppId.From(input);
        appId.GetSteamStoreUrlWithTracking(source).Should().Be(expectedUrl);
    }
}
