using Automatonymous;
using ExampleContracts.Order;
using ExampleMassTransit.Order.Api.Saga.Activity;
using System;

namespace ExampleMassTransit.Order.Api.Saga
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinishedDate { get; set; }
        public int EventNumber { get; set; }
        public byte[] RowVersion { get; set; }
        public string CurrentState { get; set; }
    }

    public class OrderStateMachine : MassTransitStateMachine<OrderSagaState>
    {
        public Event<CreateOrderEvent> CreateOrderEvent { get; set; }
        public Event<VerifyProductInCatalogResponse> VerifyProductInCatalogEvent { get; set; }
        public Event<CalculateOrderResponse> CalculateOrderEvent { get; set; }
        public Event<VerifyPriceRuleResponse> VerifyPriceRuleEvent { get; set; }
        public Event<CompletedOrderEvent> CompletedOrderEvent { get; set; }
        public State OrderCreatedState { get; set; }

        public OrderStateMachine()
        {
            Event(() => CreateOrderEvent, x => x.CorrelateById(m => m.Message.Id));
            Event(() => VerifyProductInCatalogEvent, x => x.CorrelateById(m => m.Message.CorrelationId ));
            Event(() => CalculateOrderEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => VerifyPriceRuleEvent, x => x.CorrelateById(m => m.Message.CorrelationId));

            InstanceState(x => x.CurrentState);

            Initially(When(CreateOrderEvent).Activity(x => x.OfType<CreateOrderActivity>()).TransitionTo(OrderCreatedState));
            During(OrderCreatedState, When(VerifyProductInCatalogEvent).Activity(x => x.OfType<VerifyProductInCatalogActivity>()));
            During(OrderCreatedState, When(CalculateOrderEvent).Activity(x => x.OfType<CalculateOrderActivity>()));
            During(OrderCreatedState, When(VerifyPriceRuleEvent).Activity(x => x.OfType<VerifyPriceRuleEventActivity>()));
            DuringAny(When(CompletedOrderEvent).Activity(x => x.OfType<CompletedActivity>()).Finalize());
        }
    }
}
