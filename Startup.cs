using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi;
using WeddingWebApp.Data.Abstractions;
using WeddingWebApp.Data.Cosmos;
using WeddingWebApp.Data.InMemory;
using WeddingWebApp.Libs.KeyVault;

namespace WeddingWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpLogging(options =>
            {
                options.LoggingFields =
                    HttpLoggingFields.RequestMethod |
                    HttpLoggingFields.RequestPath |
                    HttpLoggingFields.ResponseStatusCode;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            // Add services here
            IAzureKeyVaultConnectionStringStore? keyVaultStore = null;
            var keyVaultUri = Configuration["KeyVault:VaultUri"];
            if (!string.IsNullOrWhiteSpace(keyVaultUri))
            {
                var keyVaultOptions = new AzureKeyVaultOptions
                {
                    VaultUri = keyVaultUri,
                    CosmosConnectionStringSecretName = Configuration["KeyVault:CosmosConnectionStringSecretName"] ?? "CosmosConnectionString"
                };

                keyVaultStore = new AzureKeyVaultConnectionStringStore(keyVaultOptions);
                services.AddSingleton<IAzureKeyVaultConnectionStringStore>(keyVaultStore);
            }

            var cosmosConnectionString = ResolveCosmosConnectionString(Configuration);
            if (string.IsNullOrWhiteSpace(cosmosConnectionString) && keyVaultStore is not null)
            {
                cosmosConnectionString = keyVaultStore
                    .GetCosmosConnectionStringAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            if (!string.IsNullOrWhiteSpace(cosmosConnectionString))
            {
                var options = new CosmosUserClientOptions
                {
                    ConnectionString = cosmosConnectionString,
                    DatabaseId = Configuration["Cosmos:DatabaseId"] ?? "WeddingDatabase",
                    ContainerId = Configuration["Cosmos:ContainerId"] ?? "Users"
                };

                services.AddSingleton<IUserClient>(_ => new CosmosUserClient(options));
            }
            else
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var isDevelopment = string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase);
                if (!isDevelopment)
                {
                    throw new InvalidOperationException(
                        "Cosmos connection string is not configured. Set Cosmos:ConnectionString or configure Key Vault.");
                }

                services.AddSingleton<IUserClient, InMemoryUserClient>();
            }
        }

        private static string? ResolveCosmosConnectionString(IConfiguration configuration)
        {
            // 1) App settings style: Cosmos__ConnectionString -> Cosmos:ConnectionString
            var connectionString = configuration["Cosmos:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            // 2) Connection strings tab with Name = Cosmos
            connectionString = configuration.GetConnectionString("Cosmos");
            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            // 3) Connection strings tab with Name = Cosmos__ConnectionString
            connectionString = configuration.GetConnectionString("Cosmos__ConnectionString");
            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            // 4) Direct section access fallback
            connectionString = configuration["ConnectionStrings:Cosmos"];
            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            connectionString = configuration["ConnectionStrings:Cosmos__ConnectionString"];
            if (!string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            // 5) Azure App Service raw env vars from Connection strings tab.
            // DocumentDb type commonly becomes DOCDBCONNSTR_<Name>.
            var envCandidates = new[]
            {
                "DOCDBCONNSTR_Cosmos",
                "DOCDBCONNSTR_Cosmos__ConnectionString",
                "CUSTOMCONNSTR_Cosmos",
                "CUSTOMCONNSTR_Cosmos__ConnectionString"
            };

            foreach (var envName in envCandidates)
            {
                connectionString = Environment.GetEnvironmentVariable(envName);
                if (!string.IsNullOrWhiteSpace(connectionString))
                    return connectionString;
            }

            return null;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                        var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                        if (exceptionFeature?.Error is not null)
                        {
                            logger.LogError(exceptionFeature.Error, "Unhandled exception while processing request {Path}.", context.Request.Path);
                        }

                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.CompleteAsync();
                    });
                });
            }

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();

            app.UseStaticFiles();
            app.UseHttpLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
