using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi;
using WeddingWebApp.Configuration;
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

            var (cosmosConnectionString, connectionStringSource) = CosmosConfigurationResolver.ResolveConnectionString(Configuration);
            Console.WriteLine(
                $"[Startup] Cosmos connection string source: {connectionStringSource}; found: {!string.IsNullOrWhiteSpace(cosmosConnectionString)}");
            if (string.IsNullOrWhiteSpace(cosmosConnectionString) && keyVaultStore is not null)
            {
                cosmosConnectionString = keyVaultStore
                    .GetCosmosConnectionStringAsync()
                    .GetAwaiter()
                    .GetResult();
                connectionStringSource = "KeyVault";
                Console.WriteLine(
                    $"[Startup] Cosmos connection string source: {connectionStringSource}; found: {!string.IsNullOrWhiteSpace(cosmosConnectionString)}");
            }

            if (!string.IsNullOrWhiteSpace(cosmosConnectionString))
            {
                var options = new CosmosUserClientOptions
                {
                    ConnectionString = cosmosConnectionString,
                    DatabaseId = CosmosConfigurationResolver.ResolveDatabaseId(Configuration),
                    ContainerId = CosmosConfigurationResolver.ResolveContainerId(Configuration)
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
                        "Cosmos connection string is not configured. Set COSMOS__CONNECTIONSTRING or configure Key Vault.");
                }

                services.AddSingleton<IUserClient, InMemoryUserClient>();
            }
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
