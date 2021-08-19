using ExampleMassTransit.Price.Api.Consumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleMassTransit.Price.Api.Infra.Configurations
{
    public static class MasstransitConfig
    {
        public static IServiceCollection AddMassTransitConfig(this IServiceCollection services)
        {
            services.AddMassTransit(x => 
            { 
                x.AddConsumersFromNamespaceContaining<VerifyPriceRuleConsumer>();
                x.UsingRabbitMq((ctx, r) => {
                    r.ConfigureEndpoints(ctx);
                });
            });

            services.AddMassTransitHostedService();
            return services;
        }
    }
}
