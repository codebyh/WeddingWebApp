using Microsoft.Azure.Cosmos;

namespace WeddingWebApp.Security;

public sealed class CosmosAdminCredentialStore : IAdminCredentialStore, IDisposable
{
    private readonly CosmosClient cosmosClient;
    private readonly Container container;

    public CosmosAdminCredentialStore(string connectionString, string databaseId, string containerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerId);

        cosmosClient = new CosmosClient(connectionString);
        container = cosmosClient.GetContainer(databaseId, containerId);
    }

    public async Task<AdminCredential?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var normalizedUsername = AdminUserDocument.NormalizeUsername(username);
        var query = new QueryDefinition(
            "SELECT TOP 1 * FROM c WHERE c.docType = @docType AND c.username = @username")
            .WithParameter("@docType", AdminUserDocument.DocumentType)
            .WithParameter("@username", normalizedUsername);

        using var iterator = container.GetItemQueryIterator<AdminUserDocument>(query);
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            var document = page.Resource.FirstOrDefault();
            if (document is not null)
                return document.ToCredential();
        }

        return null;
    }

    public void Dispose()
    {
        cosmosClient.Dispose();
    }
}
