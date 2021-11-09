using SalePayment.Data.Repository;
using SalePayment.Domain.Outbox;

namespace SalePayment.UseCases;

public class SubmitOrder
{
    public record Command : ICommand
    {
        public Guid CustomerId { get; init; }
        public DateTime OrderDate { get; init; }
        public DateTime? RequiredDate { get; init; }
        public List<OrderDetailDto> Details { get; init; } = new();

        public string FailedAt { get; init; } = "NotFailed"; // only for test

        public record OrderSubmissionAccepted(Guid OrderId, Guid CustomerId);
        public record OrderSubmissionRejected(Guid OrderId, Guid CustomerId, string Reason);

        public record struct OrderDetailDto(Guid ProductId, decimal UnitPrice, int Quantity);

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

        internal class Handler : MutateHandlerBase<OrderOutbox>, IRequestHandler<Command, IResult>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly ITopicProducer<OrderSubmitted> _orderValidatedTopicProducer;
            private readonly ILogger<Handler> _logger;

            public Handler(IOrderRepository orderRepository,
                IRepository<OrderOutbox> orderOutboxRepository,
                ISchemaRegistryClient schemaRegistryClient,
                ITopicProducer<OrderSubmitted> orderValidatedTopicProducer,
                ILogger<Handler> logger)
                : base(schemaRegistryClient, orderOutboxRepository)
            {
                _orderRepository = orderRepository;
                _orderValidatedTopicProducer = orderValidatedTopicProducer;
                _logger = logger;
            }

            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.Log(LogLevel.Debug, "SubmitOrderHandler: {CustomerId}", request.CustomerId);

                /* for testing only */
                if (request.FailedAt.ToUpper() == "VALIDATION_FAILED")
                {
                    var rejectedModel = ResultModel<OrderSubmissionRejected>.Create(
                        new OrderSubmissionRejected(
                            NewGuid(),
                            request.CustomerId,
                            $"Test Customer cannot submit orders: {request.CustomerId}"));
                    return Results.BadRequest(rejectedModel);
                }

                /* start to persistence data and send it to outbox */
                var newOrder = await _orderRepository.AddAsync(request, cancellationToken);

                await ExportToOutbox(
                    newOrder,
                    () => (
                        new OrderCreated { Id = newOrder.Id.ToString() },
                        new OrderOutbox(),
                        "order_cdc_events"
                    ),
                    cancellationToken);
                /* end to persistence data and send it to outbox */

                /* trigger state machine to start processing the order */
                await _orderValidatedTopicProducer.Produce(new
                {
                    OrderId = newOrder.Id,
                    request.CustomerId,
                    request.OrderDate,
                    request.RequiredDate
                }, cancellationToken);

                var acceptedModel = new OrderSubmissionAccepted(newOrder.Id, request.CustomerId);
                var resultModel = ResultModel<OrderSubmissionAccepted>.Create(acceptedModel);
                return Results.Ok(resultModel);
            }
        }
    }
}
