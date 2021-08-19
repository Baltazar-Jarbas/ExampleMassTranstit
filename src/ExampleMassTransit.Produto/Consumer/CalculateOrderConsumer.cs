using ExampleContracts.Order;
using ExampleMassTransit.Product.Api.Data;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExampleMassTransit.Product.Api.Consumer
{
    public class CalculateOrderConsumer : IConsumer<CalculateOrderCommand> 
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CalculateOrderConsumer> _logger;

        public CalculateOrderConsumer(AppDbContext dbContext, ILogger<CalculateOrderConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CalculateOrderCommand> context)
        {
            _logger.LogInformation($"Calculating values with: {JsonSerializer.Serialize(context.Message)}");

            var response = new CalculateOrderResponse { CorrelationId = context.Message.CorrelationId, Items = context.Message.Items };
            context.Message.Items.ForEach(item => {
                response.Value += _dbContext.Products.FirstOrDefault(x => item.ProductId == x.Id).Value * item.Amount; 
            });

            await context.Publish(response);
        }
    }
}
