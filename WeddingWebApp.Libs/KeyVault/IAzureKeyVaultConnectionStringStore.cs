namespace WeddingWebApp.Libs.KeyVault;

public interface IAzureKeyVaultConnectionStringStore
{
    Task<string?> GetCosmosConnectionStringAsync(CancellationToken cancellationToken = default);
    Task SetCosmosConnectionStringAsync(string connectionString, CancellationToken cancellationToken = default);
}
