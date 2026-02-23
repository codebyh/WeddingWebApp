using Newtonsoft.Json;
using WeddingWebApp.Data.Models;

namespace WeddingWebApp.Data.Cosmos.Models;

internal sealed class UserListItemDocument
{
    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    [JsonProperty("displayName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonProperty("mobileNumber")]
    public string? MobileNumber { get; init; }

    [JsonProperty("email")]
    public string? Email { get; init; }

    public UserListItem ToListItem() =>
        new()
        {
            Id = Id,
            FullName = DisplayName,
            PhoneNumber = MobileNumber,
            Email = Email
        };
}
