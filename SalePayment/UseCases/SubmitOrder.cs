namespace SalePayment.UseCases;

public class SubmitOrder
{
    public record Command : ICommand
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }

        public string FailedAt { get; set; } = "NotFailed"; // only for test

        public record OrderSubmissionAccepted(Guid OrderId, Guid CustomerId);
        public record OrderSubmissionRejected(Guid OrderId, Guid CustomerId, string Reason);

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(v => v.CustomerId)
                    .NotEmpty().WithMessage("CustomerId is required.");

                RuleFor(v => v.OrderDate)
                    .NotEmpty().WithMessage("OrderDate is required.");
            }
        }

        internal class Handler : IRequestHandler<Command, IResult>
        {
            private readonly ITopicProducer<OrderSubmitted> _orderValidatedTopicProducer;
            private readonly ILogger<Handler> _logger;

            public Handler(ITopicProducer<OrderSubmitted> orderValidatedTopicProducer,
                ILogger<Handler> logger)
            {
                _orderValidatedTopicProducer = orderValidatedTopicProducer;
                _logger = logger;
            }

            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.Log(LogLevel.Debug, "SubmitOrderHandler: {CustomerId}", request.CustomerId);

                var newOrderId = NewGuid();

                // todo: gRPC check
                // validation customer_id, employee_id using gRPC
                // check order date should greater than today
                // ...

                // todo: persistence directly to database
                // create order in this service

                // todo: replication data
                // persistence ship information (event) to outbox - cdc mechanism

                // for testing only
                if (request.FailedAt.ToUpper() == "VALIDATION_FAILED")
                {
                    var rejectedModel = ResultModel<OrderSubmissionRejected>.Create(
                        new OrderSubmissionRejected(
                            newOrderId,
                            request.CustomerId,
                            $"Test Customer cannot submit orders: {request.CustomerId}"));

                    return Results.BadRequest(rejectedModel);
                }

                await _orderValidatedTopicProducer.Produce(new
                {
                    OrderId = newOrderId,
                    request.CustomerId,
                    request.OrderDate,
                    request.RequiredDate
                }, cancellationToken);

                var acceptedModel = new OrderSubmissionAccepted(newOrderId, request.CustomerId);
                var resultModel = ResultModel<OrderSubmissionAccepted>.Create(acceptedModel);

                return Results.Ok(resultModel);
            }
        }
    }
}
