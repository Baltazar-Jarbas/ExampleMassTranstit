using Domain.Core;
using System;

namespace ExampleMassTransit.Catalog.Api.Domain.Model
{
    public sealed class Catalog : Entity
    {
        public Guid ProductId  { get; set; }
        public int Amount { get; set; }
    }
}
