using System.Security.Cryptography;

namespace WeddingWebApp.Security;

public static class AdminPasswordHasher
{
    public const int DefaultIterations = 100_000;
    public const int HashSizeBytes = 32;
    public const string DefaultHashAlgorithm = "SHA256";

    public static bool VerifyPassword(
        string password,
        string expectedHashBase64,
        string saltBase64,
        int iterations,
        string hashAlgorithm)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(expectedHashBase64) ||
            string.IsNullOrWhiteSpace(saltBase64) ||
            iterations <= 0)
        {
            return false;
        }

        byte[] expectedHash;
        byte[] salt;
        try
        {
            expectedHash = Convert.FromBase64String(expectedHashBase64);
            salt = Convert.FromBase64String(saltBase64);
        }
        catch (FormatException)
        {
            return false;
        }

        HashAlgorithmName algorithmName;
        try
        {
            algorithmName = ResolveHashAlgorithm(hashAlgorithm);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithmName, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static HashAlgorithmName ResolveHashAlgorithm(string? hashAlgorithm)
    {
        if (string.IsNullOrWhiteSpace(hashAlgorithm))
            return HashAlgorithmName.SHA256;

        if (hashAlgorithm.Equals("SHA256", StringComparison.OrdinalIgnoreCase))
            return HashAlgorithmName.SHA256;

        if (hashAlgorithm.Equals("SHA384", StringComparison.OrdinalIgnoreCase))
            return HashAlgorithmName.SHA384;

        if (hashAlgorithm.Equals("SHA512", StringComparison.OrdinalIgnoreCase))
            return HashAlgorithmName.SHA512;

        throw new InvalidOperationException($"Unsupported hash algorithm: {hashAlgorithm}");
    }
}
