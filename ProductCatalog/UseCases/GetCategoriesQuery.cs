using ProductCatalog.Domain;

namespace ProductCatalog.UseCases;

public readonly record struct GetCategoriesQuery : IQuery
{
    internal class Handler : IRequestHandler<GetCategoriesQuery, IResult>
    {
        private readonly IRepository<Category> _categoryRepository;

        public Handler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IResult> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var results = await _categoryRepository.FindAsync(new NoOpSpec<Category>(), cancellationToken);
            return Results.Ok(
                ResultModel<List<CategoryDto>>.Create(results.Select(x => new CategoryDto(x.Id, x.Name)).ToList()));
        }
    }
}
