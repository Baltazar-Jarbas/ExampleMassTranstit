using Domain.Core;
using System;
using System.Collections.Generic;

namespace ExampleMassTransit.Order.Api.Domain.Model
{
    public sealed class Order : Entity
    {
        public string Customer { get; set; }
        public DateTime Timestamp { get; set; }
        public List<OrderItem> Items { get; set; }
        public bool Status { get; set; }
        public string MessageStatus { get; set; }
        public float Value { get; set; }
    }

    public sealed class OrderItem : Entity {
        public Guid OrderId { get; set; }
        public int Amount { get; set; }
        public Guid ProductId { get; set; }
        public Order Order { get; set; }
    }
}
