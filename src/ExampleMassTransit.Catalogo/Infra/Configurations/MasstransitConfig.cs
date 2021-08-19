using ExampleMassTransit.Catalog.Api.Consumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleMassTransit.Catalogo.Api.Infra.Configurations
{
    public static class MasstransitConfig
    {
        public static IServiceCollection AddMassTransitConfig(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                //x.AddConsumer<CreateCatalogConsumer>();
                x.AddConsumersFromNamespaceContaining<CreateCatalogConsumer>();
                x.UsingRabbitMq((ctx, r) => {
                    r.ConfigureEndpoints(ctx);
                });
            });

            services.AddMassTransitHostedService();
            return services;
        }
    }
}
