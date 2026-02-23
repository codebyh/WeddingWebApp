namespace WeddingWebApp.Data.Models;

public sealed class UserListItem
{
    public string Id { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
}
