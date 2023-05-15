using HumanResources.Domain;
using HumanResources.Domain.OutBox;

namespace HumanResources.UseCases;

public sealed class MutateShipper
{
    public readonly record struct CreateCommand() : ICreateCommand
    {
        public string CompanyName { get; init; } = default!;

        public Shipper ToEntity()
        {
            return new Shipper(CompanyName);
        }
    }

    internal class Handler : MutateHandlerBase<ShipperOutbox>,
        IRequestHandler<CreateCommand, IResult>
    {
        private readonly IRepository<Shipper> _repository;

        public Handler(IRepository<Shipper> repository,
            IRepository<ShipperOutbox> outboxRepository,
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
                    new ShipperCreated { Id = created.Id.ToString() },
                    new ShipperOutbox(),
                    "shipper_cdc_events"
                ),
                cancellationToken);

            return Results.Ok(ResultModel<Guid>.Create(created.Id));
        }
    }
}
