using ExampleContracts.Order;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleMassTransit.Price.Api.Consumer
{
    public class VerifyPriceRuleConsumer : IConsumer<VerifyPriceRuleCommand>
    {
        private readonly ILogger<VerifyPriceRuleConsumer> _logger;

        public VerifyPriceRuleConsumer(ILogger<VerifyPriceRuleConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VerifyPriceRuleCommand> context)
        {
            var response = new VerifyPriceRuleResponse { CorrelationId = context.Message.CorrelationId, Items = context.Message.Items };
            if (context.Message.Items.Sum(x => x.Amount) > 1)
                response.Discount = 0.1f; 
                    
           await context.Publish(response);
        }
    }
}
