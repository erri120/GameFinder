using GameFinder.StoreHandlers.EADesktop.Crypto;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class CryptoTests
{
    [Fact]
    public void Test_Decryption()
    {
        var key = new byte[]
        {
            0x01, 0xb4, 0x2f, 0x0e, 0x7e, 0x3b, 0x32, 0xe7, 0xc4, 0x25, 0x1b, 0xc3, 0x8f,
            0xa2, 0xae, 0x2e, 0xdb, 0x8d, 0xc2, 0x64, 0x98, 0xe5, 0xb7, 0x3e, 0x2a, 0x92,
            0xac, 0x9e, 0x8f, 0xfc, 0xb4, 0xf4,
        };

        var iv = Decryption.CreateDecryptionIV();

        var cipherText = File.ReadAllBytes(Path.Combine("files", "IS_erri120.encrypted"));
        var expectedPlainText = File.ReadAllText(Path.Combine("files", "IS_erri120.decrypted"));

        var plainText = Decryption.DecryptFile(cipherText, key, iv);
        plainText.Should().Be(expectedPlainText);
    }
}
