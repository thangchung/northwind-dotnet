using HumanResources.Domain;
using HumanResources.Domain.OutBox;

namespace HumanResources.UseCases;

public sealed class MutateCustomer
{
    public readonly record struct CreateCommand() : ICreateCommand
    {
        public string CompanyName { get; init; } = default!;

        public Customer ToEntity()
        {
            return new Customer(CompanyName);
        }
    }

    internal class Handler : MutateHandlerBase<CustomerOutbox>,
        IRequestHandler<CreateCommand, IResult>
    {
        private readonly IRepository<Customer> _repository;

        public Handler(IRepository<Customer> repository,
            IRepository<CustomerOutbox> outboxRepository,
            ISchemaRegistryClient  schemaRegistryClient)
            : base(schemaRegistryClient, outboxRepository)
        {
            _repository = repository;
        }

        public async Task<IResult> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var entity = request.ToEntity();
            var created = await _repository.AddAsync(entity, cancellationToken: cancellationToken);

            await ExportToOutbox(
                created,
                () => (
                    new CustomerCreated { Id = created.Id.ToString() },
                    new CustomerOutbox(),
                    "customer_cdc_events"
                ),
                cancellationToken);

            return Results.Ok(ResultModel<Guid>.Create(created.Id));
        }
    }
}
