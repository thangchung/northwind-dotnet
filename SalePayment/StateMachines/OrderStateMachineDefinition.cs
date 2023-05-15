namespace SalePayment.StateMachines;

public class OrderStateMachineDefinition : SagaDefinition<OrderState>
{
    public OrderStateMachineDefinition()
    {
        ConcurrentMessageLimit = 12;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}
