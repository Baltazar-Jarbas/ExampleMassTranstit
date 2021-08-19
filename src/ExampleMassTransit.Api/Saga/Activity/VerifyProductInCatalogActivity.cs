using Automatonymous;
using ExampleContracts.Order;
using ExampleMassTransit.Order.Api.Data;
using GreenPipes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExampleMassTransit.Order.Api.Saga.Activity
{
    public class VerifyProductInCatalogActivity : Automatonymous.Activity<OrderSagaState, VerifyProductInCatalogResponse>
    {
        private readonly AppDbContext _dbContext;
        public VerifyProductInCatalogActivity(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderSagaState, VerifyProductInCatalogResponse> context, Behavior<OrderSagaState, VerifyProductInCatalogResponse> next)
        {
            context.Instance.EventNumber = 2;
            
            var order = _dbContext.Orders.FirstOrDefault(x => x.Id == context.Data.CorrelationId);
            if (context.Data.Message != "Success")
            {
                order.Status = false;
                order.MessageStatus = context.Data.Message;

                _dbContext.SaveChanges();

                await context.Publish(new CompletedOrderEvent { CorrelationId = context.Data.CorrelationId });
            }
            else
            {
                await context.Publish(new CalculateOrderCommand { CorrelationId = context.Data.CorrelationId, Items = context.Data.Items });
            }

            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, VerifyProductInCatalogResponse, TException> context, Behavior<OrderSagaState, VerifyProductInCatalogResponse> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {

        }
    }

    public class VerifyPriceRuleEventActivity : Automatonymous.Activity<OrderSagaState, VerifyPriceRuleResponse>
    {
        private readonly AppDbContext _dbContext;
        public VerifyPriceRuleEventActivity(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderSagaState, VerifyPriceRuleResponse> context, Behavior<OrderSagaState, VerifyPriceRuleResponse> next)
        {
            var order = _dbContext.Orders.FirstOrDefault(x => x.Id == context.Data.CorrelationId);
            if (context.Data.Discount > 0)
            {
                order.Value -= (order.Value * context.Data.Discount);
                order.MessageStatus = "Apply discount";
                _dbContext.SaveChanges();
            }

            context.Instance.EventNumber = 4;

            await context.Publish(new CompletedOrderEvent { CorrelationId = context.Data.CorrelationId });

            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, VerifyPriceRuleResponse, TException> context, Behavior<OrderSagaState, VerifyPriceRuleResponse> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {

        }
    }

    public class CreateOrderActivity : Automatonymous.Activity<OrderSagaState, CreateOrderEvent>
    {
        private readonly AppDbContext _dbContext;
        public CreateOrderActivity(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderSagaState, CreateOrderEvent> context, Behavior<OrderSagaState, CreateOrderEvent> next)
        {
            _dbContext.Orders.Add(new Domain.Model.Order { 
                Id = context.Data.Id, 
                Status = true, 
                MessageStatus = "In progress", 
                Customer = context.Data.Customer, 
                Timestamp = DateTime.UtcNow, 
                Items = context.Data.Items.Select(x => new Domain.Model.OrderItem
                {
                    OrderId = context.Data.Id,
                    ProductId = x.ProductId,
                    Amount = x.Amount
                }).ToList() });

            context.Instance.InitialDate = DateTime.UtcNow;
            context.Instance.EventNumber = 1;

            _dbContext.SaveChanges();

            await context.Publish(new VerifyProductInCatalogCommand {CorrelationId = context.Data.Id, Items = context.Data.Items });
            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, CreateOrderEvent, TException> context, Behavior<OrderSagaState, CreateOrderEvent> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {

        }
    }

    public class CalculateOrderActivity : Automatonymous.Activity<OrderSagaState, CalculateOrderResponse>
    {
        private readonly AppDbContext _dbContext;
        public CalculateOrderActivity(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderSagaState, CalculateOrderResponse> context, Behavior<OrderSagaState, CalculateOrderResponse> next)
        {
            var order = _dbContext.Orders.FirstOrDefault(x => x.Id == context.Data.CorrelationId);
            order.Value = context.Data.Value;
            order.MessageStatus = "Apply Total Value";
            _dbContext.SaveChanges();
            
            context.Instance.EventNumber = 3;

            await context.Publish(new VerifyPriceRuleCommand { CorrelationId = context.Data.CorrelationId, Items = context.Data.Items });

            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, CalculateOrderResponse, TException> context, Behavior<OrderSagaState, CalculateOrderResponse> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {

        }
    }

    public class CompletedActivity : Automatonymous.Activity<OrderSagaState, CompletedOrderEvent>
    {
        private readonly AppDbContext _dbContext;

        public CompletedActivity(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderSagaState, CompletedOrderEvent> context, Behavior<OrderSagaState, CompletedOrderEvent> next)
        {
            var order = _dbContext.Orders.Include(x => x.Items).FirstOrDefault(x => x.Id == context.Data.CorrelationId);

            context.Instance.FinishedDate = DateTime.UtcNow;
            context.Instance.EventNumber = 4;

            Console.WriteLine($"Order: {JsonConvert.SerializeObject(order, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, CompletedOrderEvent, TException> context, Behavior<OrderSagaState, CompletedOrderEvent> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
