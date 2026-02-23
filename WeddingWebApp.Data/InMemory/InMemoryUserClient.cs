
using System.Collections.Concurrent;
using WeddingWebApp.Data.Abstractions;
using WeddingWebApp.Data.Ids;
using WeddingWebApp.Data.Models;

namespace WeddingWebApp.Data.InMemory;

public sealed class InMemoryUserClient : IUserClient
{
    private const string UserDocumentType = "userProfile";
    private readonly ConcurrentDictionary<string, User> users = new();

    public Task<IReadOnlyCollection<UserListItem>> GetAllSummariesAsync() =>
        Task.FromResult((IReadOnlyCollection<UserListItem>)users.Values
            .Select(user => new UserListItem
            {
                Id = user.Id,
                FullName = user.DisplayName,
                PhoneNumber = user.MobileNumber,
                Email = user.Email
            })
            .ToList());

    public Task<IReadOnlyCollection<User>> GetAllAsync() =>
        Task.FromResult((IReadOnlyCollection<User>)users.Values.ToList());

    public Task<User?> GetByIdAsync(string id)
    {
        users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        var id = GenerateUniqueId();

        // Create a new instance so we control Id/CreatedUtc if desired
        var created = new User
        {
            Id = id,
            DocType = UserDocumentType,
            DisplayName = user.DisplayName,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            Age = user.Age,
            MaritalStatus = user.MaritalStatus,
            Height = user.Height,
            Weight = user.Weight,
            BloodGroup = user.BloodGroup,
            Religion = user.Religion,
            CasteSubCaste = user.CasteSubCaste,
            MotherTongue = user.MotherTongue,
            TimeOfBirth = user.TimeOfBirth,
            PlaceOfBirth = user.PlaceOfBirth,
            StarNakshatra = user.StarNakshatra,
            Rashi = user.Rashi,
            Gothram = user.Gothram,
            Manglik = user.Manglik,
            HoroscopeCopyAttached = user.HoroscopeCopyAttached,
            HighestQualification = user.HighestQualification,
            Occupation = user.Occupation,
            CompanyName = user.CompanyName,
            JobLocation = user.JobLocation,
            AnnualIncome = user.AnnualIncome,
            FatherName = user.FatherName,
            FatherOccupation = user.FatherOccupation,
            MotherName = user.MotherName,
            MotherOccupation = user.MotherOccupation,
            Brothers = user.Brothers,
            Sisters = user.Sisters,
            FamilyStatus = user.FamilyStatus,
            MobileNumber = user.MobileNumber,
            AlternateNumber = user.AlternateNumber,
            Email = user.Email,
            Address = user.Address,
            PreferredAgeMin = user.PreferredAgeMin,
            PreferredAgeMax = user.PreferredAgeMax,
            PreferredHeight = user.PreferredHeight,
            EducationPreference = user.EducationPreference,
            LocationPreference = user.LocationPreference,
            OtherExpectations = user.OtherExpectations,
            DeclarationAccepted = user.DeclarationAccepted,
            Signature = user.Signature,
            DeclarationDate = user.DeclarationDate,
            CreatedUtc = user.CreatedUtc == default ? DateTimeOffset.UtcNow : user.CreatedUtc,
            UpdatedUtc = null
        };

        users[id] = created;
        return Task.FromResult(created);
    }

    public Task<User?> UpdateAsync(string id, User update)
    {
        // "Update logic": patch the existing user, keep CreatedUtc stable, force Id match
        if (!users.TryGetValue(id, out var existing))
            return Task.FromResult<User?>(null);

        var updated = new User
        {
            Id = id,
            DocType = UserDocumentType,
            CreatedUtc = existing.CreatedUtc,

            // Choose whether you want PATCH-like behavior or PUT-like behavior.
            // Here: PATCH-like (keep old value if update didn't send it)
            DisplayName = string.IsNullOrWhiteSpace(update.DisplayName) ? existing.DisplayName : update.DisplayName,
            BirthDate = update.BirthDate ?? existing.BirthDate,
            Gender = update.Gender ?? existing.Gender,
            Age = update.Age ?? existing.Age,
            MaritalStatus = update.MaritalStatus ?? existing.MaritalStatus,
            Height = update.Height ?? existing.Height,
            Weight = update.Weight ?? existing.Weight,
            BloodGroup = update.BloodGroup ?? existing.BloodGroup,
            Religion = update.Religion ?? existing.Religion,
            CasteSubCaste = update.CasteSubCaste ?? existing.CasteSubCaste,
            MotherTongue = update.MotherTongue ?? existing.MotherTongue,
            TimeOfBirth = update.TimeOfBirth ?? existing.TimeOfBirth,
            PlaceOfBirth = update.PlaceOfBirth ?? existing.PlaceOfBirth,
            StarNakshatra = update.StarNakshatra ?? existing.StarNakshatra,
            Rashi = update.Rashi ?? existing.Rashi,
            Gothram = update.Gothram ?? existing.Gothram,
            Manglik = update.Manglik ?? existing.Manglik,
            HoroscopeCopyAttached = update.HoroscopeCopyAttached ?? existing.HoroscopeCopyAttached,
            HighestQualification = update.HighestQualification ?? existing.HighestQualification,
            Occupation = update.Occupation ?? existing.Occupation,
            CompanyName = update.CompanyName ?? existing.CompanyName,
            JobLocation = update.JobLocation ?? existing.JobLocation,
            AnnualIncome = update.AnnualIncome ?? existing.AnnualIncome,
            FatherName = update.FatherName ?? existing.FatherName,
            FatherOccupation = update.FatherOccupation ?? existing.FatherOccupation,
            MotherName = update.MotherName ?? existing.MotherName,
            MotherOccupation = update.MotherOccupation ?? existing.MotherOccupation,
            Brothers = update.Brothers ?? existing.Brothers,
            Sisters = update.Sisters ?? existing.Sisters,
            FamilyStatus = update.FamilyStatus ?? existing.FamilyStatus,
            MobileNumber = update.MobileNumber ?? existing.MobileNumber,
            AlternateNumber = update.AlternateNumber ?? existing.AlternateNumber,
            Email = update.Email ?? existing.Email,
            Address = update.Address ?? existing.Address,
            PreferredAgeMin = update.PreferredAgeMin ?? existing.PreferredAgeMin,
            PreferredAgeMax = update.PreferredAgeMax ?? existing.PreferredAgeMax,
            PreferredHeight = update.PreferredHeight ?? existing.PreferredHeight,
            EducationPreference = update.EducationPreference ?? existing.EducationPreference,
            LocationPreference = update.LocationPreference ?? existing.LocationPreference,
            OtherExpectations = update.OtherExpectations ?? existing.OtherExpectations,
            DeclarationAccepted = update.DeclarationAccepted ?? existing.DeclarationAccepted,
            Signature = update.Signature ?? existing.Signature,
            DeclarationDate = update.DeclarationDate ?? existing.DeclarationDate,
            UpdatedUtc = DateTimeOffset.UtcNow
        };

        users[id] = updated;
        return Task.FromResult<User?>(updated);
    }

    public Task<bool> DeleteAsync(string id) =>
        Task.FromResult(users.TryRemove(id, out _));

    private string GenerateUniqueId()
    {
        while (true)
        {
            var id = UserIdGenerator.GenerateTenDigitNumericId();
            if (!users.ContainsKey(id))
                return id;
        }
    }
}

