using ProductCatalog.Domain;
using ProductCatalog.Domain.Outbox;

namespace ProductCatalog.UseCases;

public sealed class MutateProduct
{
    public record CreateCommand : ICreateCommand<ProductDto>
    {
        public string Name { get; init; } = default!;

        public Product ToProduct()
        {
            return new Product(Name, false);
        }

        internal class CreateValidator : AbstractValidator<CreateCommand>
        {
            public CreateValidator()
            {
                RuleFor(v => v.Name)
                    .NotEmpty().WithMessage("Name is required.");
            }
        }
    }

    public record UpdateCommand : IUpdateCommand<Guid, ProductDto>
    {
        public Guid Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        internal class CreateValidator : AbstractValidator<UpdateCommand>
        {
            public CreateValidator()
            {
                RuleFor(v => v.Id)
                    .NotNull().WithMessage("Id is required.");

                RuleFor(v => v.Name)
                    .NotEmpty().WithMessage("Name is required.");
            }
        }
    }

    public record DeleteCommand(Guid Id) : IDeleteCommand<Guid, bool>
    {
        internal class CreateValidator : AbstractValidator<DeleteCommand>
        {
            public CreateValidator()
            {
                RuleFor(v => v.Id)
                    .NotNull().WithMessage("Id is required.");
            }
        }
    }

    internal class Handler : MutateHandlerBase<ProductOutbox>,
        IRequestHandler<CreateCommand, ResultModel<ProductDto>>,
        IRequestHandler<UpdateCommand, ResultModel<ProductDto>>,
        IRequestHandler<DeleteCommand, ResultModel<bool>>
    {
        private readonly IRepository<Product> _productRepository;

        public Handler(IRepository<Product> productRepository,
            IRepository<ProductOutbox> productOutboxRepository, ISchemaRegistryClient  schemaRegistryClient)
            : base(schemaRegistryClient, productOutboxRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ResultModel<ProductDto>> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var entity = request.ToProduct();
            var created = await _productRepository.AddAsync(entity, cancellationToken: cancellationToken);

            await ExportToOutbox(
                created,
                () => (
                    new ProductCreated { Id = created.Id.ToString() },
                    new ProductOutbox(),
                    "product_cdc_events"
                ),
                cancellationToken);

            return ResultModel<ProductDto>.Create(
                new ProductDto(created.Id, created.Name, created.Discontinued));
        }

        public async Task<ResultModel<ProductDto>> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            var entity = await _productRepository.FindById(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new ArgumentNullException($"Couldn't find entity with id={request.Id}");
            }

            // update entity
            entity.Name = request.Name;

            var updated = await _productRepository.EditAsync(entity, cancellationToken: cancellationToken);

            await ExportToOutbox(
                updated,
                () => (
                    new ProductUpdated { Id = updated.Id.ToString() },
                    new ProductOutbox(),
                    "product_cdc_events"
                ),
                cancellationToken);

            return ResultModel<ProductDto>.Create(
                new ProductDto(updated.Id, updated.Name, updated.Discontinued));
        }

        public async Task<ResultModel<bool>> Handle(DeleteCommand request, CancellationToken cancellationToken)
        {
            var entity = await _productRepository.FindById(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new ArgumentNullException($"Couldn't find entity with id={request.Id}");
            }

            await _productRepository.DeleteAsync(entity, cancellationToken: cancellationToken);

            await ExportToOutbox(
                entity,
                () => (
                    new ProductDeleted { Id = request.Id.ToString() },
                    new ProductOutbox(),
                    "product_cdc_events"
                ),
                cancellationToken);

            return ResultModel<bool>.Create(true);
        }
    }
}
