using ExampleContracts.Product;
using ExampleMassTransit.Catalog.Api.Data;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExampleMassTransit.Catalog.Api.Consumer
{
    public class CreateCatalogConsumer : IConsumer<CreateProductCommand>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CreateCatalogConsumer> _logger;

        public CreateCatalogConsumer(AppDbContext dbContext, ILogger<CreateCatalogConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateProductCommand> context)
        {
            _logger.LogInformation($"Create a new Catalog: {JsonSerializer.Serialize(context.Message)}");
            _dbContext.Catalogs.Add(new Domain.Model.Catalog { Id = Guid.NewGuid(), ProductId = context.Message.Id, Amount = context.Message.Amount});
            await _dbContext.SaveChangesAsync();

            _logger.LogWarning($"Catalogs: {JsonSerializer.Serialize(_dbContext.Catalogs)}");
        }
    }
}
