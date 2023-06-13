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

    [Fact]
    public void Test_Decryption2()
    {
        var key = new byte[]
        {
            0x46, 0xa8, 0x96, 0x5f, 0xa5, 0xfa, 0x90, 0xcd, 0x45, 0x8b, 0xab, 0x85, 0xfe,
            0xa9, 0x40, 0xc5, 0x25, 0xd2, 0xc8, 0x6a, 0x95, 0x9f, 0xa3, 0xe1, 0x7f, 0x82,
            0xec, 0x18, 0xa3, 0x8e, 0x15, 0x86,
        };

        var iv = Decryption.CreateDecryptionIV();

        var cipherText = File.ReadAllBytes(Path.Combine("files", "IS_Nutzzz.encrypted"));
        var expectedPlainText = File.ReadAllText(Path.Combine("files", "IS_Nutzzz.decrypted"));

        var plainText = Decryption.DecryptFile(cipherText, key, iv);
        plainText.Should().Be(expectedPlainText);
    }
}
