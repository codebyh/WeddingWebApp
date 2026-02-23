using Newtonsoft.Json;

namespace WeddingWebApp.Data.Models;

public class User
{
    [JsonProperty("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString("n");

    [JsonProperty("docType")]
    public string? DocType { get; init; }

    [JsonProperty("displayName")]
    public string DisplayName { get; init; } = default!;

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
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;

    [JsonProperty("updatedUtc")]
    public DateTimeOffset? UpdatedUtc { get; init; }
}
