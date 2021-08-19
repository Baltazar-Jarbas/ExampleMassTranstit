using ExampleContracts.Order;
using ExampleContracts.Product;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExampleMassTransit.Client.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManagementController : ControllerBase 
    {
        private readonly IPublishEndpoint _publisher;

        public ManagementController(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }


        [HttpPost("Product")]
        public async Task<IActionResult> CreateProduct(CreateProductCommand request)
        {
            await _publisher.Publish(request);
            return Ok();
        }


        [HttpPost("Order")]
        public async Task<IActionResult> CreateOrder(CreateOrderEvent request)
        {
            await _publisher.Publish(request);
            return Ok();
        }
    }
}
