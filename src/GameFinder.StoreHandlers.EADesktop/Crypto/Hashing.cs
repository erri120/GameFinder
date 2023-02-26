using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SHA3.Net;

namespace GameFinder.StoreHandlers.EADesktop.Crypto;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Hashing
{
    public static string CalculateSHA1Hash(string input)
    {
        var inputSpan = input.AsSpan();

        var byteCount = Encoding.ASCII.GetByteCount(inputSpan);
        var span = byteCount < 1024 ? stackalloc byte[byteCount] : new byte[byteCount];

        Encoding.ASCII.GetBytes(inputSpan, span);
        return CalculateSHA1Hash(span);
    }

    private static string CalculateSHA1Hash(ReadOnlySpan<byte> input)
    {
        Span<byte> buffer = stackalloc byte[20];
        SHA1.HashData(input, buffer);
        return Convert.ToHexString(buffer).ToLower(CultureInfo.InvariantCulture);
    }

    public static byte[] CalculateSHA3_256Hash(string input)
    {
        var byteCount = Encoding.ASCII.GetByteCount(input);
        var buffer = new byte[byteCount];
        Encoding.ASCII.GetBytes(input.AsSpan(), buffer.AsSpan());
        return CalculateSHA3_256Hash(buffer);
    }

    private static byte[] CalculateSHA3_256Hash(byte[] input)
    {
        // TODO: find or create a lib that works with spans
        using var algorithm = Sha3.Sha3256();
        var hash = algorithm.ComputeHash(input);
        return hash;
    }
}
