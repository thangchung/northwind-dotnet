using MassTransit.Courier;
using Northwind.IntegrationEvents.Contracts;

namespace SalePayment.StateMachines.Activities;

public class ValidateOrderActivity : IActivity<OrderState, OrderValidated>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<OrderState> context)
    {
        // do something for validating order


        return context.Completed(new {OrderId = context.Arguments.OrderId});
    }

    public async Task<CompensationResult> Compensate(CompensateContext<OrderValidated> context)
    {
        await context.Publish<OrderValidatedFailed>(new
        {
            context.Log.OrderId,
            Reason = "Order Validated Faulted"
        });

        return context.Compensated();
    }
}
