using Microsoft.Azure.Cosmos;
using System.Net;
using WeddingWebApp.Data.Abstractions;
using WeddingWebApp.Data.Ids;
using WeddingWebApp.Data.Cosmos.Models;

namespace WeddingWebApp.Data.Cosmos;

public sealed class CosmosUserClient : IUserClient, IDisposable
{
    private readonly CosmosClient cosmosClient;
    private readonly CosmosUserClientOptions options;
    private readonly SemaphoreSlim initLock = new(1, 1);
    private Container? container;

    public CosmosUserClient(CosmosUserClientOptions options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new ArgumentException("Cosmos connection string is required.", nameof(options));

        this.options = options;
        cosmosClient = new CosmosClient(options.ConnectionString);
    }

    public async Task<IReadOnlyCollection<WeddingWebApp.Data.Models.UserListItem>> GetAllSummariesAsync()
    {
        var users = new List<WeddingWebApp.Data.Models.UserListItem>();
        var target = await GetContainerAsync();
        var query = new QueryDefinition(
            "SELECT c.id, c.displayName, c.mobileNumber, c.email FROM c WHERE c.docType = @docType")
            .WithParameter("@docType", UserDocument.DocumentType);
        using var iterator = target.GetItemQueryIterator<UserListItemDocument>(query);

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            users.AddRange(page.Resource.Select(x => x.ToListItem()));
        }

        return users;
    }

    public async Task<IReadOnlyCollection<WeddingWebApp.Data.Models.User>> GetAllAsync()
    {
        var users = new List<WeddingWebApp.Data.Models.User>();
        var target = await GetContainerAsync();
        var query = new QueryDefinition("SELECT * FROM c WHERE c.docType = @docType")
            .WithParameter("@docType", UserDocument.DocumentType);
        using var iterator = target.GetItemQueryIterator<UserDocument>(query);

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            users.AddRange(page.Resource.Select(x => x.ToUser()));
        }

        return users;
    }

    public async Task<WeddingWebApp.Data.Models.User?> GetByIdAsync(string id)
    {
        var target = await GetContainerAsync();
        try
        {
            var response = await target.ReadItemAsync<UserDocument>(id, new PartitionKey(id));
            return response.Resource.ToUser();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<WeddingWebApp.Data.Models.User> CreateAsync(WeddingWebApp.Data.Models.User user)
    {
        var target = await GetContainerAsync();
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var createdUser = new WeddingWebApp.Data.Models.User
            {
                Id = UserIdGenerator.GenerateTenDigitNumericId(),
                DocType = UserDocument.DocumentType,
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
                CreatedUtc = DateTimeOffset.UtcNow,
                UpdatedUtc = null
            };

            var createdDocument = UserDocument.FromUser(createdUser);
            try
            {
                await target.CreateItemAsync(createdDocument, new PartitionKey(createdDocument.Pk));
                return createdUser;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                // Retry with a new generated ID on rare collisions.
            }
        }

        throw new InvalidOperationException("Unable to generate a unique 10-digit user id.");
    }

    public async Task<WeddingWebApp.Data.Models.User?> UpdateAsync(string id, WeddingWebApp.Data.Models.User update)
    {
        var target = await GetContainerAsync();
        UserDocument? existingDocument;
        try
        {
            var read = await target.ReadItemAsync<UserDocument>(id, new PartitionKey(id));
            existingDocument = read.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            existingDocument = null;
        }

        if (existingDocument is null)
            return null;

        var existing = existingDocument.ToUser();
        var patched = new WeddingWebApp.Data.Models.User
        {
            Id = id,
            DocType = UserDocument.DocumentType,
            CreatedUtc = existing.CreatedUtc,
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

        var patchedDocument = UserDocument.FromUser(patched);
        await target.ReplaceItemAsync(patchedDocument, id, new PartitionKey(id));
        return patched;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var target = await GetContainerAsync();
        try
        {
            await target.DeleteItemAsync<UserDocument>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public void Dispose()
    {
        initLock.Dispose();
        cosmosClient.Dispose();
    }

    private async Task<Container> GetContainerAsync()
    {
        if (container is not null)
            return container;

        await initLock.WaitAsync();
        try
        {
            if (container is not null)
                return container;

            if (options.AutoCreateResources)
            {
                var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(options.DatabaseId);
                try
                {
                    var containerResponse = await database.Database.CreateContainerIfNotExistsAsync(
                        new ContainerProperties(options.ContainerId, options.PartitionKeyPath));
                    container = containerResponse.Container;
                }
                catch (CosmosException ex) when (
                    ex.StatusCode == HttpStatusCode.Conflict ||
                    ex.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Existing container may have a different partition key definition.
                    // In that case, use the existing container reference.
                    container = database.Database.GetContainer(options.ContainerId);
                }
            }
            else
            {
                container = cosmosClient.GetContainer(options.DatabaseId, options.ContainerId);
            }

            return container;
        }
        finally
        {
            initLock.Release();
        }
    }
}
