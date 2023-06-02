using GameFinder.StoreHandlers.Steam.Models;

namespace GameFinder.StoreHandlers.Steam.Tests.Models;

public class SteamAccountTypeTests
{
    [Theory]
    [InlineData(SteamAccountType.Invalid, 'I')]
    [InlineData(SteamAccountType.Individual, 'U')]
    [InlineData(SteamAccountType.Multiseat, 'M')]
    [InlineData(SteamAccountType.GameServer, 'G')]
    [InlineData(SteamAccountType.AnonGameServer, 'A')]
    [InlineData(SteamAccountType.Pending, 'P')]
    [InlineData(SteamAccountType.ContentServer, 'C')]
    [InlineData(SteamAccountType.Clan, 'g')]
    [InlineData(SteamAccountType.Chat, 'T')]
    [InlineData(SteamAccountType.AnonUser, 'a')]
    [InlineData((SteamAccountType)byte.MaxValue, '?')]
    public void Test_GetLetter(SteamAccountType accountType, char expectedLetter)
    {
        var actualLetter = accountType.GetLetter();
        actualLetter.Should().Be(expectedLetter);
    }
}
