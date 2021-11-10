using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using Grpc.Net.ClientFactory;
using SalePayment.StateMachines;

namespace SalePayment.UseCases;

public class GetOrderStateMachine
{
    public record Query : IQuery
    {
        internal class Handler : RequestHandler<Query, IResult>
        {
            private readonly GrpcClientFactory _grpcClientFactory;

            public Handler(GrpcClientFactory grpcClientFactory)
            {
                _grpcClientFactory = grpcClientFactory;
            }

            protected override IResult Handle(Query request)
            {
                var orderStateMachine = new OrderStateMachine(_grpcClientFactory);
                var graph = orderStateMachine.GetGraph();
                var generator = new StateMachineGraphvizGenerator(graph);
                var dots = generator.CreateDotFile();

                Console.WriteLine(dots);

                return Results.Ok(ResultModel<bool>.Create(true));
            }
        }
    }
}
