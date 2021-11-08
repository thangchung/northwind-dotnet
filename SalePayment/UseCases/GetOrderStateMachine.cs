using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using SalePayment.StateMachines;

namespace SalePayment.UseCases;

public class GetOrderStateMachine
{
    public record Query : IQuery
    {
        internal class Handler : RequestHandler<Query, IResult>
        {
            protected override IResult Handle(Query request)
            {
                var orderStateMachine = new OrderStateMachine();
                var graph = orderStateMachine.GetGraph();
                var generator = new StateMachineGraphvizGenerator(graph);
                var dots = generator.CreateDotFile();

                Console.WriteLine(dots);

                return Results.Ok(ResultModel<bool>.Create(true));
            }
        }
    }
}
