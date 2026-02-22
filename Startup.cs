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
                try
                {
                    cosmosConnectionString = keyVaultStore
                        .GetCosmosConnectionStringAsync()
                        .GetAwaiter()
                        .GetResult();
                    connectionStringSource = "KeyVault";
                    Console.WriteLine(
                        $"[Startup] Cosmos connection string source: {connectionStringSource}; found: {!string.IsNullOrWhiteSpace(cosmosConnectionString)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Startup] Failed to load Cosmos connection string from Key Vault: {ex.Message}");
                }
            }

            if (!string.IsNullOrWhiteSpace(cosmosConnectionString))
            {
                try
                {
                    var options = new CosmosUserClientOptions
                    {
                        ConnectionString = cosmosConnectionString,
                        DatabaseId = CosmosConfigurationResolver.ResolveDatabaseId(Configuration),
                        ContainerId = CosmosConfigurationResolver.ResolveContainerId(Configuration)
                    };

                    services.AddSingleton<IUserClient>(_ => new CosmosUserClient(options));
                    Console.WriteLine(
                        $"[Startup] Cosmos client configured. DatabaseId={options.DatabaseId}; ContainerId={options.ContainerId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Startup] Failed to configure Cosmos client. Falling back to in-memory store: {ex.Message}");
                    services.AddSingleton<IUserClient, InMemoryUserClient>();
                }
            }
            else
            {
                Console.WriteLine("[Startup] Cosmos connection string not found. Falling back to in-memory store.");
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
                            Console.Error.WriteLine($"[Unhandled] Path={context.Request.Path}; TraceId={context.TraceIdentifier}");
                            Console.Error.WriteLine(exceptionFeature.Error.ToString());
                        }

                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Internal Server Error",
                            traceId = context.TraceIdentifier
                        });
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
