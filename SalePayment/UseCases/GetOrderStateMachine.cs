using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using Grpc.Net.ClientFactory;
using SalePayment.StateMachines;

namespace SalePayment.UseCases;

public record struct GetOrderStateMachineQuery : IQuery
{
    internal class Handler : RequestHandler<GetOrderStateMachineQuery, IResult>
    {
        // private readonly GrpcClientFactory _grpcClientFactory;
        private readonly ILoggerFactory _loggerFactory;

        public Handler(/*GrpcClientFactory grpcClientFactory,*/ ILoggerFactory loggerFactory)
        {
            // _grpcClientFactory = grpcClientFactory;
            _loggerFactory = loggerFactory;
        }

        protected override IResult Handle(GetOrderStateMachineQuery request)
        {
            var orderStateMachine = new OrderStateMachine(/*_grpcClientFactory,*/ _loggerFactory);
            var graph = orderStateMachine.GetGraph();
            var generator = new StateMachineGraphvizGenerator(graph);
            var dots = generator.CreateDotFile();

            Console.WriteLine(dots);

            return Results.Ok(ResultModel<string>.Create(dots));
        }
    }
}
