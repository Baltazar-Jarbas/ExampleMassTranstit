using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleContracts.Order
{
    public class CreateOrderEvent 
    {
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Customer { get; set; }
        public List<OrderItem> Items { get; set; }
    }
    
    public class OrderItem
    {
        [JsonIgnore]
        public Guid OrderId { get; set; }
        public int Amount { get; set; }
        public Guid ProductId { get; set; }
    }

    public class VerifyProductInCatalogCommand
    {
        public Guid CorrelationId { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class CalculateOrderCommand
    {
        public Guid CorrelationId { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class VerifyPriceRuleCommand
    {
        public Guid CorrelationId { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class VerifyProductInCatalogResponse
    {
        public Guid CorrelationId { get; set; }
        public string Message { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class VerifyPriceRuleResponse
    {
        public Guid CorrelationId { get; set; }
        public float Discount { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class CalculateOrderResponse 
    {
        public Guid CorrelationId { get; set; }
        public float Value { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class CompletedOrderEvent
    {
        public Guid CorrelationId { get; set; }
    }
}
