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

        var displayName = NormalizeRequired(input.DisplayName);
        if (string.IsNullOrWhiteSpace(displayName))
        {
            error = "Full name is required.";
            return false;
        }

        createUser = new User
        {
            // Always server-generated on create.
            Id = string.Empty,
            CreatedUtc = default,
            UpdatedUtc = null,
            DisplayName = displayName,
            BirthDate = input.BirthDate,
            Gender = NormalizeOptional(input.Gender),
            Age = input.Age,
            MaritalStatus = NormalizeOptional(input.MaritalStatus),
            Height = NormalizeOptional(input.Height),
            Weight = NormalizeOptional(input.Weight),
            BloodGroup = NormalizeOptional(input.BloodGroup),
            Religion = NormalizeOptional(input.Religion),
            CasteSubCaste = NormalizeOptional(input.CasteSubCaste),
            MotherTongue = NormalizeOptional(input.MotherTongue),
            TimeOfBirth = NormalizeOptional(input.TimeOfBirth),
            PlaceOfBirth = NormalizeOptional(input.PlaceOfBirth),
            StarNakshatra = NormalizeOptional(input.StarNakshatra),
            Rashi = NormalizeOptional(input.Rashi),
            Gothram = NormalizeOptional(input.Gothram),
            Manglik = input.Manglik,
            HoroscopeCopyAttached = input.HoroscopeCopyAttached,
            HighestQualification = NormalizeOptional(input.HighestQualification),
            Occupation = NormalizeOptional(input.Occupation),
            CompanyName = NormalizeOptional(input.CompanyName),
            JobLocation = NormalizeOptional(input.JobLocation),
            AnnualIncome = NormalizeOptional(input.AnnualIncome),
            FatherName = NormalizeOptional(input.FatherName),
            FatherOccupation = NormalizeOptional(input.FatherOccupation),
            MotherName = NormalizeOptional(input.MotherName),
            MotherOccupation = NormalizeOptional(input.MotherOccupation),
            Brothers = input.Brothers,
            Sisters = input.Sisters,
            FamilyStatus = NormalizeOptional(input.FamilyStatus),
            MobileNumber = NormalizeOptional(input.MobileNumber),
            AlternateNumber = NormalizeOptional(input.AlternateNumber),
            Email = NormalizeOptional(input.Email),
            Address = NormalizeOptional(input.Address),
            PreferredAgeMin = input.PreferredAgeMin,
            PreferredAgeMax = input.PreferredAgeMax,
            PreferredHeight = NormalizeOptional(input.PreferredHeight),
            EducationPreference = NormalizeOptional(input.EducationPreference),
            LocationPreference = NormalizeOptional(input.LocationPreference),
            OtherExpectations = NormalizeOptional(input.OtherExpectations),
            DeclarationAccepted = input.DeclarationAccepted,
            Signature = NormalizeOptional(input.Signature),
            DeclarationDate = input.DeclarationDate
        };

        return true;
    }

    private static string NormalizeRequired(string? value) =>
        (value ?? string.Empty).Trim();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
