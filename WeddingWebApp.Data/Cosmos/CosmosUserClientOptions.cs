namespace WeddingWebApp.Data.Cosmos;

public sealed class CosmosUserClientOptions
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseId { get; init; } = "WeddingDatabase";
    public string ContainerId { get; init; } = "Users";
    public string PartitionKeyPath { get; init; } = "/pk";
    public bool AutoCreateResources { get; init; } = true;
}
