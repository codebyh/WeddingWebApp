using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace WeddingWebApp.Libs.KeyVault;

public sealed class AzureKeyVaultConnectionStringStore : IAzureKeyVaultConnectionStringStore
{
    private readonly SecretClient secretClient;
    private readonly string cosmosConnectionStringSecretName;

    public AzureKeyVaultConnectionStringStore(AzureKeyVaultOptions options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.VaultUri))
            throw new ArgumentException("Key Vault URI is required.", nameof(options));
        if (string.IsNullOrWhiteSpace(options.CosmosConnectionStringSecretName))
            throw new ArgumentException("Cosmos connection string secret name is required.", nameof(options));

        cosmosConnectionStringSecretName = options.CosmosConnectionStringSecretName;
        secretClient = new SecretClient(new Uri(options.VaultUri), new DefaultAzureCredential());
    }

    public async Task<string?> GetCosmosConnectionStringAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var secret = await secretClient.GetSecretAsync(cosmosConnectionStringSecretName, cancellationToken: cancellationToken);
            return secret.Value.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task SetCosmosConnectionStringAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));

        await secretClient.SetSecretAsync(cosmosConnectionStringSecretName, connectionString, cancellationToken);
    }
}
