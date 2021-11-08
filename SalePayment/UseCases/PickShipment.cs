namespace SalePayment.UseCases;

public class PickShipment
{
    public record Command : ICommand
    {
        public Guid OrderId { get; set; }
        public Guid ShipperId { get; init; }
        public Guid CustomerId { get; init; }
        public string? BeFailedAt { get; init; } // "Dispatched"

        public record ShipmentSubmissionAccepted(Guid OrderId, Guid ShipperId, Guid CustomerId);
        public record ShipmentSubmissionRejected(Guid OrderId, Guid ShipperId, Guid CustomerId, string Reason);

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.OrderId)
                    .NotEmpty().WithMessage("OrderId is required.");

                RuleFor(v => v.ShipperId)
                    .NotEmpty().WithMessage("ShipperId is required.");

                RuleFor(v => v.CustomerId)
                    .NotEmpty().WithMessage("CustomerId is required.");
            }
        }

        internal class Handler : IRequestHandler<Command, IResult>
        {
            private readonly ITopicProducer<ShipmentDispatched> _shipmentDispatchedTopicProducer;
            private readonly ITopicProducer<ShipmentDispatchedFailed> _shipmentDispatchedFailedTopicProducer;
            private readonly ITopicProducer<ShipmentCancelled> _shipmentCancelledTopicProducer;

            public Handler(ITopicProducer<ShipmentDispatched> shipmentDispatchedTopicProducer,
                ITopicProducer<ShipmentDispatchedFailed> shipmentDispatchedFailedTopicProducer,
                ITopicProducer<ShipmentCancelled> shipmentCancelledTopicProducer)
            {
                _shipmentDispatchedTopicProducer = shipmentDispatchedTopicProducer;
                _shipmentDispatchedFailedTopicProducer = shipmentDispatchedFailedTopicProducer;
                _shipmentCancelledTopicProducer = shipmentCancelledTopicProducer;
            }

            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                // for testing only
                if (request.BeFailedAt?.ToUpper() == "DISPATCHED")
                {
                    await _shipmentDispatchedFailedTopicProducer.Produce(new {request.OrderId}, cancellationToken);
                    await _shipmentCancelledTopicProducer.Produce(new {request.OrderId}, cancellationToken);

                    var rejectedModel = ResultModel<ShipmentSubmissionRejected>.Create(
                        new ShipmentSubmissionRejected(
                            request.OrderId,
                            request.ShipperId,
                            request.CustomerId,
                            $"Test cannot processing payment: {request.OrderId}"));

                    return Results.BadRequest(rejectedModel);
                }

                await _shipmentDispatchedTopicProducer.Produce(new {request.OrderId}, cancellationToken);

                var resultModel = ResultModel<ShipmentSubmissionAccepted>.Create(
                    new ShipmentSubmissionAccepted(
                        request.OrderId,
                        request.ShipperId,
                        request.CustomerId));

                return Results.Ok(resultModel);
            }
        }
    }
}
