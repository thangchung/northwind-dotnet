using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using Grpc.Net.ClientFactory;
using SalePayment.StateMachines;

namespace SalePayment.UseCases;

public record struct GetOrderStateMachineQuery : IQuery
{
    internal class Handler : RequestHandler<GetOrderStateMachineQuery, IResult>
    {
        private readonly GrpcClientFactory _grpcClientFactory;

        public Handler(GrpcClientFactory grpcClientFactory)
        {
            _grpcClientFactory = grpcClientFactory;
        }

        protected override IResult Handle(GetOrderStateMachineQuery request)
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
