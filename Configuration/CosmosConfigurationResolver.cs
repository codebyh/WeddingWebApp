namespace WeddingWebApp.Configuration;

public static class CosmosConfigurationResolver
{
    private const string ConnectionStringKey = "Cosmos:ConnectionString";
    private const string DatabaseIdKey = "Cosmos:DatabaseId";
    private const string ContainerIdKey = "Cosmos:ContainerId";
    private const string ConnectionStringsCosmosKey = "ConnectionStrings:Cosmos";
    private const string ConnectionStringsCosmosSingleUnderscoreKey = "ConnectionStrings:COSMOS_CONNECTIONSTRING";

    private const string CustomConnStrConnectionStringEnv = "CUSTOMCONNSTR_COSMOS__CONNECTIONSTRING";
    private const string CustomConnStrConnectionStringSingleUnderscoreEnv = "CUSTOMCONNSTR_COSMOS_CONNECTIONSTRING";
    private const string DocDbConnStrConnectionStringEnv = "DOCDBCONNSTR_COSMOS__CONNECTIONSTRING";
    private const string DocDbConnStrConnectionStringSingleUnderscoreEnv = "DOCDBCONNSTR_COSMOS_CONNECTIONSTRING";

    private const string CustomConnStrDatabaseIdEnv = "CUSTOMCONNSTR_COSMOS__DATABASEID";
    private const string CustomConnStrDatabaseIdSingleUnderscoreEnv = "CUSTOMCONNSTR_COSMOS_DATABASEID";
    private const string DocDbConnStrDatabaseIdEnv = "DOCDBCONNSTR_COSMOS__DATABASEID";
    private const string DocDbConnStrDatabaseIdSingleUnderscoreEnv = "DOCDBCONNSTR_COSMOS_DATABASEID";

    private const string CustomConnStrContainerIdEnv = "CUSTOMCONNSTR_COSMOS__CONTAINERID";
    private const string CustomConnStrContainerIdSingleUnderscoreEnv = "CUSTOMCONNSTR_COSMOS_CONTAINERID";
    private const string DocDbConnStrContainerIdEnv = "DOCDBCONNSTR_COSMOS__CONTAINERID";
    private const string DocDbConnStrContainerIdSingleUnderscoreEnv = "DOCDBCONNSTR_COSMOS_CONTAINERID";

    public const string DefaultDatabaseId = "WeddingDatabase";
    public const string DefaultContainerId = "Users";

    public static (string? Value, string Source) ResolveConnectionString(IConfiguration configuration)
    {
        var (connectionString, source) = ResolveFirstEnvironmentValue(
            CustomConnStrConnectionStringEnv,
            CustomConnStrConnectionStringSingleUnderscoreEnv,
            DocDbConnStrConnectionStringEnv,
            DocDbConnStrConnectionStringSingleUnderscoreEnv);
        if (!string.IsNullOrWhiteSpace(connectionString))
            return (connectionString, source!);

        connectionString = configuration[ConnectionStringsCosmosKey];
        if (!string.IsNullOrWhiteSpace(connectionString))
            return (connectionString, ConnectionStringsCosmosKey);

        connectionString = configuration[ConnectionStringsCosmosSingleUnderscoreKey];
        if (!string.IsNullOrWhiteSpace(connectionString))
            return (connectionString, ConnectionStringsCosmosSingleUnderscoreKey);

        connectionString = configuration[ConnectionStringKey];
        if (!string.IsNullOrWhiteSpace(connectionString))
            return (connectionString, $"{ConnectionStringKey} (env: COSMOS__CONNECTIONSTRING)");

        return (null, "not found");
    }

    public static string ResolveDatabaseId(IConfiguration configuration)
    {
        var (databaseId, _) = ResolveFirstEnvironmentValue(
            CustomConnStrDatabaseIdEnv,
            CustomConnStrDatabaseIdSingleUnderscoreEnv,
            DocDbConnStrDatabaseIdEnv,
            DocDbConnStrDatabaseIdSingleUnderscoreEnv);
        if (!string.IsNullOrWhiteSpace(databaseId))
            return databaseId;

        databaseId = configuration[DatabaseIdKey];
        return string.IsNullOrWhiteSpace(databaseId) ? DefaultDatabaseId : databaseId;
    }

    public static string ResolveContainerId(IConfiguration configuration)
    {
        var (containerId, _) = ResolveFirstEnvironmentValue(
            CustomConnStrContainerIdEnv,
            CustomConnStrContainerIdSingleUnderscoreEnv,
            DocDbConnStrContainerIdEnv,
            DocDbConnStrContainerIdSingleUnderscoreEnv);
        if (!string.IsNullOrWhiteSpace(containerId))
            return containerId;

        containerId = configuration[ContainerIdKey];
        return string.IsNullOrWhiteSpace(containerId) ? DefaultContainerId : containerId;
    }

    private static (string? Value, string? Source) ResolveFirstEnvironmentValue(params string[] names)
    {
        foreach (var name in names)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrWhiteSpace(value))
                return (value, name);
        }

        return (null, null);
    }
}
