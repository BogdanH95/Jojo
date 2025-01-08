using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
          {
              configure.AddConsumers(Assembly.GetEntryAssembly());

              configure.UsingRabbitMq((context, cfg) =>
              {
                  var configuration = context.GetService<IConfiguration>() ?? throw new ArgumentNullException(nameof(IConfiguration));

                  var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>() ?? throw new ArgumentNullException(nameof(ServiceSettings));
                  var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>() ?? throw new ArgumentNullException(nameof(MongoDbSettings));
                  var rabbitMqSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>() ?? throw new ArgumentNullException(nameof(RabbitMQSettings));
                  cfg.Host(rabbitMqSettings.Host);
                  cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
              });
          });
            return services;
        }

    }
}