using GameFinder.StoreHandlers.EADesktop.Crypto;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class CryptoTests
{
    [Theory]
    [InlineData("erri120", "3f37d8cece4441299a6abbd8db2acc0102cfd398")]
    public void Test_SHA1(string input, string expectedOutput)
    {
        var actualOutput = Hashing.CalculateSHA1Hash(input);
        actualOutput.Should().Be(expectedOutput);
    }
}
