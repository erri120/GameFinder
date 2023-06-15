using GameFinder.StoreHandlers.Steam.Models;

namespace GameFinder.StoreHandlers.Steam.Tests.Models;

public class SteamIdTests
{
    [Theory]
    [InlineData(
        76561198092541763,
        SteamUniverse.Public,
        SteamAccountType.Individual,
        132276035,
        66138017,
        "STEAM_1:1:66138017",
        "[U:1:132276035]")]
    [InlineData(
        76561198110222274,
        SteamUniverse.Public,
        SteamAccountType.Individual,
        149956546,
        74978273,
        "STEAM_1:0:74978273",
        "[U:1:149956546]")]
    public void Test_SteamId(
        ulong input,
        SteamUniverse expectedUniverse,
        SteamAccountType expectedAccountType,
        uint expectedAccountId,
        uint expectedAccountNumber,
        string expectedSteam2Id,
        string expectedSteam3Id)
    {
        var steamId = new SteamId(input);

        steamId.Universe.Should().Be(expectedUniverse);
        steamId.AccountType.Should().Be(expectedAccountType);
        steamId.AccountId.Should().Be(expectedAccountId);
        steamId.AccountNumber.Should().Be(expectedAccountNumber);
        steamId.Steam2Id.Should().Be(expectedSteam2Id);
        steamId.Steam3Id.Should().Be(expectedSteam3Id);

        steamId.ProfileUrl.Should().EndWith($"/{input}");
        steamId.Steam3IdProfileUrl.Should().EndWith($"/{expectedSteam3Id}");

        steamId.ToString().Should().Be(expectedSteam3Id);
    }

    [Theory]
    [InlineData(
        149956546,
        76561193815254978,
        74978273,
        "STEAM_1:0:74978273",
        "[U:1:149956546]")]
    public void Test_FromAccountId(
        uint input,
        ulong expectedRawId,
        uint expectedAccountNumber,
        string expectedSteam2Id,
        string expectedSteam3Id)
    {
        var steamId = SteamId.FromAccountId(input);

        steamId.RawId.Should().Be(expectedRawId);
        steamId.Universe.Should().Be(SteamUniverse.Public);
        steamId.AccountType.Should().Be(SteamAccountType.Individual);
        steamId.AccountId.Should().Be(input);
        steamId.AccountNumber.Should().Be(expectedAccountNumber);
        steamId.Steam2Id.Should().Be(expectedSteam2Id);
        steamId.Steam3Id.Should().Be(expectedSteam3Id);
    }
}
