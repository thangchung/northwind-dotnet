using System.Linq.Expressions;

namespace ProductCatalog.Domain.Specs;

public class GetCategoriesSpec : SpecificationBase<Category>
{
    public override Expression<Func<Category, bool>> Criteria => p => true;
}
