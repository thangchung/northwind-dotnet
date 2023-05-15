using HumanResources.Domain;
using HumanResources.Domain.OutBox;

namespace HumanResources.UseCases;

public sealed class MutateEmployee
{
    public readonly record struct CreateCommand() : ICreateCommand
    {
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;

        public Employee ToEntity()
        {
            return new Employee(FirstName, LastName);
        }
    }

    internal class Handler : MutateHandlerBase<EmployeeOutbox>,
        IRequestHandler<CreateCommand, IResult>
    {
        private readonly IRepository<Employee> _repository;

        public Handler(IRepository<Employee> repository,
            IRepository<EmployeeOutbox> outboxRepository,
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
                    new EmployeeCreated { Id = created.Id.ToString() },
                    new EmployeeOutbox(),
                    "employee_cdc_events"
                ),
                cancellationToken);

            return Results.Ok(ResultModel<Guid>.Create(created.Id));
        }
    }
}
