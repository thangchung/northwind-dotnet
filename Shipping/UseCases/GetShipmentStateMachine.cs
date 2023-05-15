using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using Grpc.Net.ClientFactory;
using Shipping.StateMachines;

namespace Shipping.UseCases;

public record struct GetShipmentStateMachineQuery : IQuery
{
    internal class Handler : RequestHandler<GetShipmentStateMachineQuery, IResult>
    {
        // private readonly GrpcClientFactory _grpcClientFactory;
        private readonly ILoggerFactory _loggerFactory;

        public Handler(/*GrpcClientFactory grpcClientFactory,*/ ILoggerFactory loggerFactory)
        {
            // _grpcClientFactory = grpcClientFactory;
            _loggerFactory = loggerFactory;
        }

        protected override IResult Handle(GetShipmentStateMachineQuery request)
        {
            var orderStateMachine = new ShipmentStateMachine(/*_grpcClientFactory,*/ _loggerFactory);
            var graph = orderStateMachine.GetGraph();
            var generator = new StateMachineGraphvizGenerator(graph);
            var dots = generator.CreateDotFile();

            Console.WriteLine(dots);

            return Results.Ok(ResultModel<string>.Create(dots));
        }
    }
}
