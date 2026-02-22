using Newtonsoft.Json;

namespace WeddingWebApp.Data.Cosmos.Models;

internal sealed class UserDocument
{
    [JsonProperty("id")]
    public string Id { get; init; } = string.Empty;

    [JsonProperty("pk")]
    public string Pk { get; init; } = string.Empty;

    [JsonProperty("displayName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonProperty("birthDate")]
    public DateOnly? BirthDate { get; init; }

    [JsonProperty("gender")]
    public string? Gender { get; init; }

    [JsonProperty("age")]
    public int? Age { get; init; }

    [JsonProperty("maritalStatus")]
    public string? MaritalStatus { get; init; }

    [JsonProperty("height")]
    public string? Height { get; init; }

    [JsonProperty("weight")]
    public string? Weight { get; init; }

    [JsonProperty("bloodGroup")]
    public string? BloodGroup { get; init; }

    [JsonProperty("religion")]
    public string? Religion { get; init; }

    [JsonProperty("casteSubCaste")]
    public string? CasteSubCaste { get; init; }

    [JsonProperty("motherTongue")]
    public string? MotherTongue { get; init; }

    [JsonProperty("timeOfBirth")]
    public string? TimeOfBirth { get; init; }

    [JsonProperty("placeOfBirth")]
    public string? PlaceOfBirth { get; init; }

    [JsonProperty("starNakshatra")]
    public string? StarNakshatra { get; init; }

    [JsonProperty("rashi")]
    public string? Rashi { get; init; }

    [JsonProperty("gothram")]
    public string? Gothram { get; init; }

    [JsonProperty("manglik")]
    public bool? Manglik { get; init; }

    [JsonProperty("horoscopeCopyAttached")]
    public bool? HoroscopeCopyAttached { get; init; }

    [JsonProperty("highestQualification")]
    public string? HighestQualification { get; init; }

    [JsonProperty("occupation")]
    public string? Occupation { get; init; }

    [JsonProperty("companyName")]
    public string? CompanyName { get; init; }

    [JsonProperty("jobLocation")]
    public string? JobLocation { get; init; }

    [JsonProperty("annualIncome")]
    public string? AnnualIncome { get; init; }

    [JsonProperty("fatherName")]
    public string? FatherName { get; init; }

    [JsonProperty("fatherOccupation")]
    public string? FatherOccupation { get; init; }

    [JsonProperty("motherName")]
    public string? MotherName { get; init; }

    [JsonProperty("motherOccupation")]
    public string? MotherOccupation { get; init; }

    [JsonProperty("brothers")]
    public int? Brothers { get; init; }

    [JsonProperty("sisters")]
    public int? Sisters { get; init; }

    [JsonProperty("familyStatus")]
    public string? FamilyStatus { get; init; }

    [JsonProperty("mobileNumber")]
    public string? MobileNumber { get; init; }

    [JsonProperty("alternateNumber")]
    public string? AlternateNumber { get; init; }

    [JsonProperty("email")]
    public string? Email { get; init; }

    [JsonProperty("address")]
    public string? Address { get; init; }

    [JsonProperty("preferredAgeMin")]
    public int? PreferredAgeMin { get; init; }

    [JsonProperty("preferredAgeMax")]
    public int? PreferredAgeMax { get; init; }

    [JsonProperty("preferredHeight")]
    public string? PreferredHeight { get; init; }

    [JsonProperty("educationPreference")]
    public string? EducationPreference { get; init; }

    [JsonProperty("locationPreference")]
    public string? LocationPreference { get; init; }

    [JsonProperty("otherExpectations")]
    public string? OtherExpectations { get; init; }

    [JsonProperty("declarationAccepted")]
    public bool? DeclarationAccepted { get; init; }

    [JsonProperty("signature")]
    public string? Signature { get; init; }

    [JsonProperty("declarationDate")]
    public DateOnly? DeclarationDate { get; init; }

    [JsonProperty("createdUtc")]
    public DateTimeOffset CreatedUtc { get; init; }

    [JsonProperty("updatedUtc")]
    public DateTimeOffset? UpdatedUtc { get; init; }

    public WeddingWebApp.Data.Models.User ToUser() =>
        new()
        {
            Id = Id,
            DisplayName = DisplayName,
            BirthDate = BirthDate,
            Gender = Gender,
            Age = Age,
            MaritalStatus = MaritalStatus,
            Height = Height,
            Weight = Weight,
            BloodGroup = BloodGroup,
            Religion = Religion,
            CasteSubCaste = CasteSubCaste,
            MotherTongue = MotherTongue,
            TimeOfBirth = TimeOfBirth,
            PlaceOfBirth = PlaceOfBirth,
            StarNakshatra = StarNakshatra,
            Rashi = Rashi,
            Gothram = Gothram,
            Manglik = Manglik,
            HoroscopeCopyAttached = HoroscopeCopyAttached,
            HighestQualification = HighestQualification,
            Occupation = Occupation,
            CompanyName = CompanyName,
            JobLocation = JobLocation,
            AnnualIncome = AnnualIncome,
            FatherName = FatherName,
            FatherOccupation = FatherOccupation,
            MotherName = MotherName,
            MotherOccupation = MotherOccupation,
            Brothers = Brothers,
            Sisters = Sisters,
            FamilyStatus = FamilyStatus,
            MobileNumber = MobileNumber,
            AlternateNumber = AlternateNumber,
            Email = Email,
            Address = Address,
            PreferredAgeMin = PreferredAgeMin,
            PreferredAgeMax = PreferredAgeMax,
            PreferredHeight = PreferredHeight,
            EducationPreference = EducationPreference,
            LocationPreference = LocationPreference,
            OtherExpectations = OtherExpectations,
            DeclarationAccepted = DeclarationAccepted,
            Signature = Signature,
            DeclarationDate = DeclarationDate,
            CreatedUtc = CreatedUtc,
            UpdatedUtc = UpdatedUtc
        };

    public static UserDocument FromUser(WeddingWebApp.Data.Models.User user) =>
        new()
        {
            Id = user.Id,
            Pk = user.Id,
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
            CreatedUtc = user.CreatedUtc,
            UpdatedUtc = user.UpdatedUtc
        };
}
