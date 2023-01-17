using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace GameFinder.StoreHandlers.EADesktop.Crypto;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Decryption
{
    private const string AllUsersGenericId = "allUsersGenericId";
    private const string IS = "IS";

    private static readonly byte[] preComputedIV =
    {
        0x84, 0xef, 0xc4, 0xb8, 0x36, 0x11, 0x9c, 0x20, 0x41, 0x93, 0x98, 0xc3, 0xf3,
        0xf2, 0xbc, 0xef,
    };

    public static byte[] CreateDecryptionKey(IHardwareInfoProvider hardwareInfoProvider)
    {
        var hardwareString = HardwareInformation.GenerateHardwareString(hardwareInfoProvider);

        var hardwareHash = Hashing.CalculateSHA1Hash(hardwareString);
        var hashInput = AllUsersGenericId + IS + hardwareHash;
        var key = Hashing.CalculateSHA3_256Hash(hashInput);

        return key;
    }

    public static byte[] CreateDecryptionIV()
    {
        // NOTE: they calculate a 256-bit hash, but only use the first 16 bytes for AES

        // const string hashInput = AllUsersGenericId + IS;
        // var iv = new byte[16];
        //
        // var hash = Hashing.CalculateSHA3_256Hash(hashInput);
        // var span = hash.AsSpan();
        // var slice = span[..16];
        // slice.CopyTo(iv.AsSpan());
        //
        // return iv;
        return preComputedIV;
    }

    public static string DecryptFile(byte[] fileContents, byte[] key, byte[] iv)
    {
        // skips the first 64 bytes, because they contain a hash we don't need
        using var cipherTextStream = new MemoryStream(fileContents, 64, fileContents.Length - 64, writable: false);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(key, iv);
        using var cryptoStream = new CryptoStream(cipherTextStream, decryptor, CryptoStreamMode.Read);
        using var decryptionStream = new StreamReader(cryptoStream);
        var plainText = decryptionStream.ReadToEnd();

        return plainText;
    }
}
