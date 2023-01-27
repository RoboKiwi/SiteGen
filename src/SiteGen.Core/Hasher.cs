using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace SiteGen.Core;

public class Hasher
{
    static readonly HashAlgorithm sha256 = SHA256.Create();
    static readonly HashAlgorithm md5 = MD5.Create();
    static readonly Encoding encoding = Encoding.UTF8;

    public static string BytesToHex(ReadOnlySpan<byte> span, int startIndex, int length)
    {
        const string hexValues = "0123456789abcdef";

        var result = ArrayPool<char>.Shared.Rent(length * 2);

        try
        {
            var src = span.Slice(startIndex, length);

            int i = 0;
            int j = 0;

            while (i < src.Length)
            {
                var b = src[i++];
                result[j++] = hexValues[b >> 4];
                result[j++] = hexValues[b & 0xF];
            }

            return result.AsSpan(0).ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(result);
        }
    }

    static string Hash(HashAlgorithm hasher, string value)
    {
        var result = HashBytes(hasher, value);
        return BytesToHex(result, 0, result.Length);
    }

    static ReadOnlySpan<byte> HashBytes(HashAlgorithm hasher, string value)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(value.Length * 2);
        var hash = ArrayPool<byte>.Shared.Rent(hasher.HashSize);

        try
        {
            var bytesRead = encoding.GetBytes(value.AsSpan(), buffer.AsSpan());

            if (hasher.TryComputeHash(buffer.AsSpan(0, bytesRead), hash.AsSpan(), out var bytesWritten))
            {
                return hash.AsSpan(0, bytesWritten);
            }

            throw new InvalidOperationException("Couldn't generate hash");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            ArrayPool<byte>.Shared.Return(hash);
        }
    }

    public static string Sha256Hash(string value)
    {
        return Hash(sha256, value);
    }
    public static string Md5Hash(string value)
    {
        return Hash(md5, value);
    }

    public static ReadOnlySpan<byte> Md5Bytes(string value)
    {
        return HashBytes(md5, value);
    }
}
