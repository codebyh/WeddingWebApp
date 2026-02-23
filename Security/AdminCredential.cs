namespace WeddingWebApp.Security;

public sealed record AdminCredential(
    string Username,
    string PasswordHash,
    string PasswordSalt,
    int PasswordIterations,
    string PasswordHashAlgorithm,
    bool IsActive);
