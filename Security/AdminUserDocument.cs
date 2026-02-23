using Newtonsoft.Json;

namespace WeddingWebApp.Security;

internal sealed class AdminUserDocument
{
    public const string DocumentType = "adminUser";

    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    [JsonProperty("pk")]
    public string Pk { get; init; } = string.Empty;

    [JsonProperty("docType")]
    public string DocType { get; init; } = DocumentType;

    [JsonProperty("username")]
    public string Username { get; init; } = string.Empty;

    [JsonProperty("passwordHash")]
    public string PasswordHash { get; init; } = string.Empty;

    [JsonProperty("passwordSalt")]
    public string PasswordSalt { get; init; } = string.Empty;

    [JsonProperty("passwordIterations")]
    public int PasswordIterations { get; init; } = AdminPasswordHasher.DefaultIterations;

    [JsonProperty("passwordHashAlgorithm")]
    public string PasswordHashAlgorithm { get; init; } = AdminPasswordHasher.DefaultHashAlgorithm;

    [JsonProperty("isActive")]
    public bool IsActive { get; init; } = true;

    [JsonProperty("createdUtc")]
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;

    public AdminCredential ToCredential() =>
        new(
            string.IsNullOrWhiteSpace(Username) ? string.Empty : NormalizeUsername(Username),
            PasswordHash,
            PasswordSalt,
            PasswordIterations <= 0 ? AdminPasswordHasher.DefaultIterations : PasswordIterations,
            string.IsNullOrWhiteSpace(PasswordHashAlgorithm) ? AdminPasswordHasher.DefaultHashAlgorithm : PasswordHashAlgorithm,
            IsActive);

    public static string NormalizeUsername(string username) =>
        string.IsNullOrWhiteSpace(username)
            ? throw new ArgumentException("Username is required.", nameof(username))
            : username.Trim().ToLowerInvariant();

}
