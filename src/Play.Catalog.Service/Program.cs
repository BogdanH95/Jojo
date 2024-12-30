using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Scalar.AspNetCore;

namespace Play.Catalog.Service.Entities
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Configure the HTTP request pipeline.
            if (host.Environment.IsDevelopment())
            {
                host.MapOpenApi();
                host.UseSwaggerUI(optioons =>
                {
                    optioons.SwaggerEndpoint("/openapi/v1.json", "Demo Api");
                });
                host.MapScalarApiReference(options =>
                {
                    options.WithTitle("Demo Api")
                            .WithTheme(ScalarTheme.DeepSpace)
                            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }
            host.UseHttpsRedirection();
            host.MapControllers();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}