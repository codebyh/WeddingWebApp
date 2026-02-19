using Newtonsoft.Json;

namespace WeddingWebApp.Data.Models;

public class PublicUser
{
    [JsonProperty("name")]
    public string Name { get; init; } = default!;

    [JsonProperty("gender")]
    public string? Gender { get; init; }
}
