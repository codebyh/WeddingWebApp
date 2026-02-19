using WeddingWebApp.Data.Models;

namespace WeddingWebApp.Validation;

public static class AdminUserCreateValidation
{
    public static bool TryBuildCreateUser(User? input, out User createUser, out string? error)
    {
        createUser = default!;
        error = null;

        if (input is null)
        {
            error = "Request body is required.";
            return false;
        }

        var displayName = (input.DisplayName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(displayName))
        {
            error = "Display name is required.";
            return false;
        }

        createUser = new User
        {
            // Always server-generated on create.
            Id = string.Empty,
            CreatedUtc = default,
            DisplayName = displayName,
            BirthDate = input.BirthDate,
            Gender = NormalizeOptional(input.Gender),
            Email = NormalizeOptional(input.Email)
        };

        return true;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
