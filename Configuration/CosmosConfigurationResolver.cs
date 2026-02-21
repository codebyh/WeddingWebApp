namespace WeddingWebApp.Configuration;

public static class CosmosConfigurationResolver
{
    private const string ConnectionStringKey = "Cosmos:ConnectionString";
    private const string DatabaseIdKey = "Cosmos:DatabaseId";
    private const string ContainerIdKey = "Cosmos:ContainerId";

    public const string DefaultDatabaseId = "WeddingDatabase";
    public const string DefaultContainerId = "Users";

    public static (string? Value, string Source) ResolveConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration[ConnectionStringKey];
        if (!string.IsNullOrWhiteSpace(connectionString))
            return (connectionString, $"{ConnectionStringKey} (env: COSMOS__CONNECTIONSTRING)");

        return (null, "not found");
    }

    public static string ResolveDatabaseId(IConfiguration configuration)
    {
        var databaseId = configuration[DatabaseIdKey];
        return string.IsNullOrWhiteSpace(databaseId) ? DefaultDatabaseId : databaseId;
    }

    public static string ResolveContainerId(IConfiguration configuration)
    {
        var containerId = configuration[ContainerIdKey];
        return string.IsNullOrWhiteSpace(containerId) ? DefaultContainerId : containerId;
    }
}
