using System;
using System.Text.Json.Serialization;

namespace ExampleContracts.Product
{
    public class CreateProductCommand
    {
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public float Value { get; set; }
        public int Amount { get; set; }
    }
}
