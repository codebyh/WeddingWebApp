using Microsoft.Azure.Cosmos;
using System.Net;
using WeddingWebApp.Data.Abstractions;
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

    public async Task<IReadOnlyCollection<WeddingWebApp.Data.Models.User>> GetAllAsync()
    {
        var users = new List<WeddingWebApp.Data.Models.User>();
        var target = await GetContainerAsync();
        using var iterator = target.GetItemQueryIterator<UserDocument>("SELECT * FROM c");

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
        var createdUser = new WeddingWebApp.Data.Models.User
        {
            Id = Guid.NewGuid().ToString("n"),
            DisplayName = user.DisplayName,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            Email = user.Email,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        var createdDocument = UserDocument.FromUser(createdUser);
        await target.CreateItemAsync(createdDocument, new PartitionKey(createdDocument.Pk));
        return createdUser;
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
            CreatedUtc = existing.CreatedUtc,
            DisplayName = string.IsNullOrWhiteSpace(update.DisplayName) ? existing.DisplayName : update.DisplayName,
            BirthDate = update.BirthDate ?? existing.BirthDate,
            Gender = update.Gender ?? existing.Gender,
            Email = update.Email ?? existing.Email
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
