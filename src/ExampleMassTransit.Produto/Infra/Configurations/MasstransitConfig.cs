using ExampleMassTransit.Product.Api.Consumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleMassTransit.Product.Api.Infra.Configurations
{
    public static class MasstransitConfig
    {
        public static IServiceCollection AddMassTransitConfig(this IServiceCollection services)
        {
            services.AddMassTransit(x => 
            { 
                x.AddConsumersFromNamespaceContaining<CreateProductConsumer>();
                x.UsingRabbitMq((ctx, r) => {
                    r.ConfigureEndpoints(ctx);
                });
            });

            services.AddMassTransitHostedService();
            return services;
        }
    }
}
