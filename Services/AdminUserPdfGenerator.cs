using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WeddingWebApp.Data.Models;

namespace WeddingWebApp.Services;

public static class AdminUserPdfGenerator
{
    static AdminUserPdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Generate(User user)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("Bride and Groom Profile").Bold().FontSize(18);
                    col.Item().Text($"Profile Id: {user.Id}").FontColor(Colors.Grey.Darken2);
                });

                page.Content().Column(col =>
                {
                    col.Spacing(4);

                    AddSection(col, "1. Personal Details",
                    [
                        ("Full Name", user.DisplayName),
                        ("Gender", user.Gender),
                        ("Date Of Birth", FormatDate(user.BirthDate)),
                        ("Age", FormatInt(user.Age)),
                        ("Marital Status", user.MaritalStatus),
                        ("Height", user.Height),
                        ("Weight", user.Weight),
                        ("Blood Group", user.BloodGroup),
                        ("Religion", user.Religion),
                        ("Caste / Sub Caste", user.CasteSubCaste),
                        ("Mother Tongue", user.MotherTongue)
                    ]);

                    AddSection(col, "2. Horoscope Details",
                    [
                        ("Time Of Birth", user.TimeOfBirth),
                        ("Place Of Birth", user.PlaceOfBirth),
                        ("Star (Nakshatra)", user.StarNakshatra),
                        ("Rashi", user.Rashi),
                        ("Gothram", user.Gothram),
                        ("Manglik", FormatBool(user.Manglik)),
                        ("Horoscope Copy Attached", FormatBool(user.HoroscopeCopyAttached))
                    ]);

                    AddSection(col, "3. Education & Career",
                    [
                        ("Highest Qualification", user.HighestQualification),
                        ("Occupation", user.Occupation),
                        ("Company Name", user.CompanyName),
                        ("Job Location", user.JobLocation),
                        ("Annual Income", user.AnnualIncome)
                    ]);

                    AddSection(col, "4. Family Details",
                    [
                        ("Father Name", user.FatherName),
                        ("Father Occupation", user.FatherOccupation),
                        ("Mother Name", user.MotherName),
                        ("Mother Occupation", user.MotherOccupation),
                        ("Brothers", FormatInt(user.Brothers)),
                        ("Sisters", FormatInt(user.Sisters)),
                        ("Family Status", user.FamilyStatus)
                    ]);

                    AddSection(col, "5. Contact Details",
                    [
                        ("Mobile Number", user.MobileNumber),
                        ("Alternate Number", user.AlternateNumber),
                        ("Email", user.Email),
                        ("Address", user.Address)
                    ]);

                    AddSection(col, "6. Partner Preferences",
                    [
                        ("Preferred Age Min", FormatInt(user.PreferredAgeMin)),
                        ("Preferred Age Max", FormatInt(user.PreferredAgeMax)),
                        ("Preferred Height", user.PreferredHeight),
                        ("Education Preference", user.EducationPreference),
                        ("Location Preference", user.LocationPreference),
                        ("Other Expectations", user.OtherExpectations)
                    ]);

                    AddSection(col, "7. Declaration",
                    [
                        ("Declaration Accepted", FormatBool(user.DeclarationAccepted)),
                        ("Signature", user.Signature),
                        ("Declaration Date", FormatDate(user.DeclarationDate))
                    ]);

                    AddSection(col, "Audit",
                    [
                        ("Created UTC", user.CreatedUtc.ToString("u")),
                        ("Updated UTC", user.UpdatedUtc?.ToString("u"))
                    ]);
                });
            });
        }).GeneratePdf();
    }

    private static void AddSection(ColumnDescriptor column, string title, IEnumerable<(string Label, string? Value)> items)
    {
        column.Item().PaddingTop(8).Text(title).Bold().FontSize(13);

        foreach (var (label, value) in items)
        {
            if (string.IsNullOrWhiteSpace(value))
                continue;

            column.Item().Row(row =>
            {
                row.ConstantItem(180).Text(label).SemiBold();
                row.RelativeItem().Text(value);
            });
        }

        column.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
    }

    private static string? FormatDate(DateOnly? value) =>
        value?.ToString("yyyy-MM-dd");

    private static string? FormatInt(int? value) =>
        value?.ToString();

    private static string? FormatBool(bool? value)
    {
        if (value is null)
            return null;

        return value.Value ? "Yes" : "No";
    }
}
