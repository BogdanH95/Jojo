using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Play.Catalog.Service.Settings;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.String));

            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>() ?? throw new ArgumentNullException(nameof(IConfiguration));
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>() ?? throw new ArgumentNullException(nameof(ServiceSettings));
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>() ?? throw new ArgumentNullException(nameof(MongoDbSettings));
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
            services.AddSingleton<IRepository<Item>>(serviceProvider =>
            {
                var db = serviceProvider.GetService<IMongoDatabase>() ?? throw new ArgumentNullException(nameof(IMongoDatabase));
                return new MongoRepository<Item>(db, "items");
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {
            services.AddSingleton(serviceProvider =>
            {
                var db = serviceProvider.GetService<IMongoDatabase>() ?? throw new ArgumentNullException(nameof(IMongoDatabase));
                return new MongoRepository<T>(db, collectionName);
            });

            return services;
        }
    }
}