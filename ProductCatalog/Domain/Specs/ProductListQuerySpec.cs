namespace ProductCatalog.Domain.Specs;

public sealed class EntityListQuerySpec<TEntity, TResponse> : GridSpecificationBase<TEntity>
    where TEntity : notnull
    where TResponse : notnull
{
    public EntityListQuerySpec(IListQuery<ListResultModel<TResponse>> gridQueryInput)
    {
        ApplyIncludeList(gridQueryInput.Includes);

        ApplyFilterList(gridQueryInput.Filters);

        ApplySortingList(gridQueryInput.Sorts);

        ApplyPaging(gridQueryInput.Page, gridQueryInput.PageSize);
    }
}
