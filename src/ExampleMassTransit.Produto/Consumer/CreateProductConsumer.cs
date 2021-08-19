using ExampleContracts.Product;
using ExampleMassTransit.Product.Api.Data;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExampleMassTransit.Product.Api.Consumer
{
    public class CreateProductConsumer : IConsumer<CreateProductCommand>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CreateProductConsumer> _logger;

        public CreateProductConsumer(AppDbContext dbContext, ILogger<CreateProductConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateProductCommand> context)
        {
            _logger.LogInformation($"Create a new Product: {JsonSerializer.Serialize(context.Message)}");
            _dbContext.Products.Add(new Domain.Model.Product { Id = context.Message.Id, Name = context.Message.Name, Value = context.Message.Value });
            await _dbContext.SaveChangesAsync();

            _logger.LogWarning($"Products: {JsonSerializer.Serialize(_dbContext.Products)}");
        }
    }
}
