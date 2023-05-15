using HumanResources.Domain;
using HumanResources.Domain.OutBox;

namespace HumanResources.UseCases;

public sealed class MutateSupplier
{
    public readonly record struct CreateCommand() : ICreateCommand
    {
        public string CompanyName { get; init; } = default!;

        public Supplier ToEntity()
        {
            return new Supplier(CompanyName);
        }
    }

    internal class Handler : MutateHandlerBase<SupplierOutbox>,
        IRequestHandler<CreateCommand, IResult>
    {
        private readonly IRepository<Supplier> _supplierRepository;

        public Handler(IRepository<Supplier> supplierRepository,
            IRepository<SupplierOutbox> supplierOutboxRepository,
            ISchemaRegistryClient  schemaRegistryClient)
            : base(schemaRegistryClient, supplierOutboxRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IResult> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var entity = request.ToEntity();
            var created = await _supplierRepository.AddAsync(entity, cancellationToken: cancellationToken);

            await ExportToOutbox(
                created,
                () => (
                    new SupplierCreated { Id = created.Id.ToString() },
                    new SupplierOutbox(),
                    "supplier_cdc_events"
                ),
                cancellationToken);

            return Results.Ok(ResultModel<Guid>.Create(created.Id));
        }
    }
}
