namespace WeddingWebApp.Libs.KeyVault;

public sealed class AzureKeyVaultOptions
{
    public string VaultUri { get; init; } = string.Empty;
    public string CosmosConnectionStringSecretName { get; init; } = "CosmosConnectionString";
}
