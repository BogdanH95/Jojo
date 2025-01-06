using System.ComponentModel.DataAnnotations;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

namespace Play.Inventory.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongo()
                .AddMongoRepository<InventoryItem>("inventoryItems");

            Random jitterer = new Random();

            services.AddHttpClient<CatalogClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5000");
            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}.");
                }
                ))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(15)
                ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1))

            ;

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddSwaggerGen();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API v1"));

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}