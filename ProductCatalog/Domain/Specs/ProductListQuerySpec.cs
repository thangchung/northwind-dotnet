namespace ProductCatalog.Domain.Specs;

public sealed class EntityListQuerySpec<TEntity> : GridSpecificationBase<TEntity>
    where TEntity : notnull
{
    public EntityListQuerySpec(IListQuery gridQueryInput)
    {
        ApplyIncludeList(gridQueryInput.Includes);

        ApplyFilterList(gridQueryInput.Filters);

        ApplySortingList(gridQueryInput.Sorts);

        ApplyPaging(gridQueryInput.Page, gridQueryInput.PageSize);
    }
}
