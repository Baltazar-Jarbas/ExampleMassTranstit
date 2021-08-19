using ExampleMassTransit.Order.Api.Saga;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleMassTransit.Order.Api.Infra.Configurations
{
    public static class MasstransitConfig
    {
        public static IServiceCollection AddMassTransitConfig(this IServiceCollection services)
        {
            services.AddMassTransit(x => {
                x.AddSagaStateMachine<OrderStateMachine, OrderSagaState>().InMemoryRepository();
                x.UsingRabbitMq((ctx, cfg) => 
                {
                    cfg.ConfigureEndpoints(ctx);
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}
