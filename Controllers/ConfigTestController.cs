using Microsoft.AspNetCore.Mvc;
using WeddingWebApp.Configuration;

namespace WeddingWebApp.Controllers;

[ApiController]
[Route("api/test/config")]
public class ConfigTestController : ControllerBase
{
    private readonly IConfiguration configuration;

    public ConfigTestController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpGet("cosmos")]
    public IActionResult GetCosmosConfig()
    {
        var (connectionString, source) = CosmosConfigurationResolver.ResolveConnectionString(configuration);
        var databaseId = CosmosConfigurationResolver.ResolveDatabaseId(configuration);
        var containerId = CosmosConfigurationResolver.ResolveContainerId(configuration);

        return Ok(new
        {
            connectionStringResolved = !string.IsNullOrWhiteSpace(connectionString),
            connectionStringSource = source,
            connectionStringLength = connectionString?.Length ?? 0,
            databaseId,
            containerId
        });
    }
}
