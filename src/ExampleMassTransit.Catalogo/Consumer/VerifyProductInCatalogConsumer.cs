using ExampleContracts.Order;
using ExampleMassTransit.Catalog.Api.Data;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleMassTransit.Catalog.Api.Consumer
{
    public class VerifyProductInCatalogConsumer : IConsumer<VerifyProductInCatalogCommand>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<VerifyProductInCatalogConsumer> _logger;

        public VerifyProductInCatalogConsumer(AppDbContext dbContext, ILogger<VerifyProductInCatalogConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VerifyProductInCatalogCommand> context)
        {
            var productsId = context.Message.Items.Select(i => i.ProductId).ToList();
            var entities = _dbContext.Catalogs.Where(x => productsId.Contains(x.ProductId)).ToList();

            if(entities.Any(x => context.Message.Items.All(i => i.ProductId == x.ProductId && x.Amount >= i.Amount)))
            {
                entities.ForEach(item =>
                {
                    var request = context.Message.Items.First(x => x.ProductId == item.ProductId);
                    item.Amount = item.Amount >= request.Amount ? item.Amount - request.Amount : item.Amount;
                });

                _dbContext.SaveChanges();

                await context.Publish(new VerifyProductInCatalogResponse { CorrelationId = context.Message.CorrelationId, Message = "Success", Items = context.Message.Items });
            }
            else
                await context.Publish(new VerifyProductInCatalogResponse { CorrelationId = context.Message.CorrelationId, Message = "Algum produto não possui estoque", Items = context.Message.Items });
            

        }
    }
}
