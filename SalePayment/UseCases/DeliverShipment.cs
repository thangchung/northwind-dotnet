namespace SalePayment.UseCases;

public class DeliverShipment
{
    public record Command : ICommand
    {
        public Guid OrderId { get; set; }
        public Guid ShipperId { get; set; }
        public Guid CustomerId { get; set; }
        public string BeFailedAt { get; set; } // "Delivered"

        public record DeliverySubmissionAccepted(Guid OrderId, Guid ShipperId, Guid CustomerId);
        public record DeliverySubmissionRejected(Guid OrderId, Guid ShipperId, Guid CustomerId, string Reason);

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
            private readonly ITopicProducer<ShipmentDelivered> _shipmentDeliveredTopicProducer;
            private readonly ITopicProducer<ShipmentDeliveredFailed> _shipmentDeliveredFailedTopicProducer;
            private readonly ITopicProducer<OrderCompleted> _orderCompletedTopicProducer;
            private readonly ITopicProducer<ShipmentCancelled> _shipmentCancelledTopicProducer;

            public Handler(ITopicProducer<ShipmentDelivered> shipmentDeliveredTopicProducer,
                ITopicProducer<ShipmentDeliveredFailed> shipmentDeliveredFailedTopicProducer,
                ITopicProducer<OrderCompleted> orderCompletedTopicProducer,
                ITopicProducer<ShipmentCancelled> shipmentCancelledTopicProducer)
            {
                _shipmentDeliveredTopicProducer = shipmentDeliveredTopicProducer;
                _shipmentDeliveredFailedTopicProducer = shipmentDeliveredFailedTopicProducer;
                _orderCompletedTopicProducer = orderCompletedTopicProducer;
                _shipmentCancelledTopicProducer = shipmentCancelledTopicProducer;
            }

            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                // for testing only
                if (request.BeFailedAt == "DELIVERED")
                {
                    await _shipmentDeliveredFailedTopicProducer.Produce(new {request.OrderId}, cancellationToken);
                    await _shipmentCancelledTopicProducer.Produce(new {request.OrderId}, cancellationToken);

                    var rejectedModel = ResultModel<DeliverySubmissionRejected>.Create(
                        new DeliverySubmissionRejected(
                            request.OrderId,
                            request.ShipperId,
                            request.CustomerId,
                            $"Test cannot deliver the packages: {request.OrderId}"));

                    return Results.BadRequest(rejectedModel);
                }

                await _shipmentDeliveredTopicProducer.Produce(new {request.OrderId}, cancellationToken);
                await _orderCompletedTopicProducer.Produce(new {request.OrderId}, cancellationToken);

                var resultModel = ResultModel<DeliverySubmissionAccepted>.Create(
                    new DeliverySubmissionAccepted(
                        request.OrderId,
                        request.ShipperId,
                        request.CustomerId));

                return Results.Ok(resultModel);
            }
        }
    }
}
