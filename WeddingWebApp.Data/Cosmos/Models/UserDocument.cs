using Newtonsoft.Json;

namespace WeddingWebApp.Data.Cosmos.Models;

internal sealed class UserDocument
{
    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    [JsonProperty("displayName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonProperty("birthDate")]
    public DateOnly? BirthDate { get; init; }

    [JsonProperty("gender")]
    public string? Gender { get; init; }

    [JsonProperty("email")]
    public string? Email { get; init; }

    [JsonProperty("createdUtc")]
    public DateTimeOffset CreatedUtc { get; init; }

    public WeddingWebApp.Data.Models.User ToUser() =>
        new()
        {
            Id = Id,
            DisplayName = DisplayName,
            BirthDate = BirthDate,
            Gender = Gender,
            Email = Email,
            CreatedUtc = CreatedUtc
        };

    public static UserDocument FromUser(WeddingWebApp.Data.Models.User user) =>
        new()
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            Email = user.Email,
            CreatedUtc = user.CreatedUtc
        };
}
