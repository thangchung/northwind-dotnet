using System.Linq.Expressions;
using ProductCatalog.Domain;
using ProductCatalog.Domain.Outbox;

namespace ProductCatalog.UseCases;

public struct MutateProduct
{
    public record GetQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<Product>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
                Includes.Add(x => x.Category);
                Includes.Add(x => x.SupplierInfo);
            }

            public override Expression<Func<Product, bool>> Criteria => x => x.Id == _id;
        }

        internal class GetValidator : AbstractValidator<GetQuery>
        {
            public GetValidator()
            {
                RuleFor(v => v.Id)
                    .NotEmpty().WithMessage("Id is required.");
            }
        }
    }

    public record CreateCommand : ICreateCommand
    {
        public string Name { get; init; } = default!;
        public Guid? CategoryId { get; init; }

        public Product ToProduct()
        {
            var product = new Product(Name, false);
            if (CategoryId is not null)
            {
                product.AssignCategory(CategoryId.Value);
            }
            return product;
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

    public record UpdateCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public string Name { get; set; } = default!;
        public Guid? CategoryId { get; init; }

        internal class UpdateValidator : AbstractValidator<UpdateCommand>
        {
            public UpdateValidator()
            {
                RuleFor(v => v.Id)
                    .NotNull().WithMessage("Id is required.");

                RuleFor(v => v.Name)
                    .NotEmpty().WithMessage("Name is required.");
            }
        }
    }

    public record DeleteCommand : IDeleteCommand<Guid>
    {
        public Guid Id { get; init; }

        internal class DeleteValidator : AbstractValidator<DeleteCommand>
        {
            public DeleteValidator()
            {
                RuleFor(v => v.Id)
                    .NotNull().WithMessage("Id is required.");
            }
        }
    }

    internal class Handler : MutateHandlerBase<ProductOutbox>,
        IRequestHandler<GetQuery, IResult>,
        IRequestHandler<CreateCommand, IResult>,
        IRequestHandler<UpdateCommand, IResult>,
        IRequestHandler<DeleteCommand, IResult>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public Handler(IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<ProductOutbox> productOutboxRepository, ISchemaRegistryClient  schemaRegistryClient)
            : base(schemaRegistryClient, productOutboxRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IResult> Handle(GetQuery request, CancellationToken cancellationToken)
        {
            var item = await _productRepository.FindOneAsync(new GetQuery.GetSpec(request.Id), cancellationToken);
            if (item is null)
            {
                throw new Exception($"Couldn't find item={request.Id}");
            }

            var result = new ProductDto(item.Id, item.Name, item.Discontinued);

            if (item.Category is not null)
            {
                result.AssignCategoryName(item.Category.Id, item.Category.Name);
            }

            return Results.Ok(ResultModel<ProductDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            var entity = request.ToProduct();
            var created = await _productRepository.AddAsync(entity, cancellationToken: cancellationToken);

            var @event = new ProductCreated {ProductId = created.Id.ToString(), ProductName = created.Name};
            var result = new ProductDto(created.Id, created.Name, created.Discontinued);
            if (request.CategoryId is not null)
            {
                var category = await _categoryRepository.FindById(request.CategoryId.Value, cancellationToken);

                @event.CategoryId = category.Id.ToString();
                @event.CategoryName = category.Name;

                result.AssignCategoryName(category.Id, category.Name);
            }

            await ExportToOutbox(
                created,
                () => (
                    @event,
                    new ProductOutbox(),
                    "product_cdc_events"
                ),
                cancellationToken);

            return Results.Ok(ResultModel<ProductDto>.Create(result));
        }

        public async Task<IResult> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            var entity = await _productRepository.FindById(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new ArgumentNullException($"Couldn't find entity with id={request.Id}");
            }

            // update entity
            entity.Name = request.Name;

            var @event = new ProductUpdated {ProductId = request.Id.ToString(), ProductName = request.Name};
            var result = new ProductDto(request.Id, request.Name, false);
            if (request.CategoryId is not null)
            {
                var category = await _categoryRepository.FindById(request.CategoryId.Value, cancellationToken);

                entity.AssignCategory(request.CategoryId.Value);

                @event.CategoryId = category.Id.ToString();
                @event.CategoryName = category.Name;

                result.AssignCategoryName(category.Id, category.Name);
            }

            var updated = await _productRepository.EditAsync(entity, cancellationToken: cancellationToken);

            await ExportToOutbox(
                updated,
                () => (
                    @event,
                    new ProductOutbox(),
                    "product_cdc_events"
                ),
                cancellationToken);

            return Results.Ok(ResultModel<ProductDto>.Create(result));
        }

        public async Task<IResult> Handle(DeleteCommand request, CancellationToken cancellationToken)
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

            return Results.Ok(ResultModel<bool>.Create(true));
        }
    }
}
