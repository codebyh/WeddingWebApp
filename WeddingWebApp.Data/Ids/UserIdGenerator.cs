using System.Security.Cryptography;

namespace WeddingWebApp.Data.Ids;

public static class UserIdGenerator
{
    public static string GenerateTenDigitNumericId()
    {
        Span<byte> bytes = stackalloc byte[10];
        RandomNumberGenerator.Fill(bytes);

        Span<char> chars = stackalloc char[10];
        chars[0] = (char)('1' + (bytes[0] % 9));

        for (var i = 1; i < chars.Length; i++)
            chars[i] = (char)('0' + (bytes[i] % 10));

        return new string(chars);
    }
}
