using Domain.Core;

namespace ExampleMassTransit.Product.Api.Domain.Model
{
    public sealed class Product : Entity
    {
        public string Name { get; set; }
        public float Value { get; set; }
    }

}
