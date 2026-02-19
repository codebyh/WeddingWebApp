using Newtonsoft.Json;

namespace WeddingWebApp.Data.Models;

public class User
{
    [JsonProperty("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString("n");

    [JsonProperty("displayName")]
    public string DisplayName { get; init; } = default!;

    [JsonProperty("birthDate")]
    public DateOnly? BirthDate { get; init; }

    [JsonProperty("gender")]
    public string? Gender { get; init; }

    [JsonProperty("email")]
    public string? Email { get; init; }

    [JsonProperty("createdUtc")]
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;
}

