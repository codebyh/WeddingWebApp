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

            var cosmosConnectionString = Configuration["Cosmos:ConnectionString"];
            if (string.IsNullOrWhiteSpace(cosmosConnectionString) && keyVaultStore is not null)
            {
                cosmosConnectionString = keyVaultStore
                    .GetCosmosConnectionStringAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            if (!string.IsNullOrWhiteSpace(cosmosConnectionString))
            {
                var useAutoCreate = true;
                if (bool.TryParse(Configuration["Cosmos:AutoCreateResources"], out var configuredAutoCreate))
                {
                    useAutoCreate = configuredAutoCreate;
                }

                var options = new CosmosUserClientOptions
                {
                    ConnectionString = cosmosConnectionString,
                    DatabaseId = Configuration["Cosmos:DatabaseId"] ?? "WeddingDatabase",
                    ContainerId = Configuration["Cosmos:ContainerId"] ?? "Users",
                    PartitionKeyPath = Configuration["Cosmos:PartitionKeyPath"] ?? "/pk",
                    AutoCreateResources = useAutoCreate
                };

                services.AddSingleton<IUserClient>(_ => new CosmosUserClient(options));
            }
            else
            {
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

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
