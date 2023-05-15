namespace SalePayment.UseCases;

public record ProcessPaymentCommand : ICommand
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public string TransactionId { get; init; } = null!;
    public string? FailedAt { get; set; } = "NotFailed"; // testing only

    public readonly record struct PaymentSubmissionAccepted(Guid OrderId, Guid CustomerId);

    public readonly record struct PaymentSubmissionRejected(Guid OrderId, Guid CustomerId, string Reason);

    internal class Validator : AbstractValidator<ProcessPaymentCommand>
    {
        public Validator()
        {
            RuleFor(v => v.OrderId)
                .NotEmpty().WithMessage("OrderId is required.");

            RuleFor(v => v.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(v => v.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");
        }
    }

    internal class Handler : IRequestHandler<ProcessPaymentCommand, IResult>
    {
        private readonly ITopicProducer<PaymentProcessed> _paymentProcessedTopicProducer;
        private readonly ITopicProducer<PaymentProcessedFailed> _paymentProcessedFailedTopicProducer;

        public Handler(ITopicProducer<PaymentProcessed> paymentProcessedTopicProducer,
            ITopicProducer<PaymentProcessedFailed> paymentProcessedFailedTopicProducer)
        {
            _paymentProcessedTopicProducer = paymentProcessedTopicProducer;
            _paymentProcessedFailedTopicProducer = paymentProcessedFailedTopicProducer;
        }

        public async Task<IResult> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            // for testing only
            if (request.FailedAt?.ToUpper() == "PAYMENT_FAILED")
            {
                await _paymentProcessedFailedTopicProducer.Produce(new {request.OrderId}, cancellationToken);

                var rejectedModel = ResultModel<PaymentSubmissionRejected>.Create(
                    new PaymentSubmissionRejected(
                        request.OrderId,
                        request.CustomerId,
                        $"Test cannot processing payment: {request.OrderId}"));

                return Results.BadRequest(rejectedModel);
            }

            await _paymentProcessedTopicProducer.Produce(
                new {request.OrderId, request.CustomerId, request.TransactionId}, cancellationToken);

            // todo: send notification to shipper hub (list of order need to ship)
            // todo: in there some of shipper will pickup the ship order to ship
            // ...

            var resultModel = ResultModel<PaymentSubmissionAccepted>.Create(
                new PaymentSubmissionAccepted(
                    request.OrderId,
                    request.CustomerId));

            return Results.Ok(resultModel);
        }
    }
}
