using Skinet.Core.Entites;

namespace Skinet.Core.Specifications;

public class ProductFilterSortPaginationSpecification:BaseSpecification<Product>
{
    public ProductFilterSortPaginationSpecification(ProductSpecParams specParams) : base(product =>
        (specParams.Brands.Count==0 || specParams.Brands.Contains(product.Brand)) &&
        (specParams.Types.Count==0 || specParams.Types.Contains(product.Type))
    )
    {
        ApplyPagination(specParams.PageSize*(specParams.PageIndex-1), specParams.PageSize);
        switch (specParams.SortBy)
        {
            case "priceAsc":
                SetOrderBy(p=>p.Price);
                break;
            case "priceDesc":
                SetOrderByDescending(p => p.Price);
                break;
            default:
                SetOrderBy(p => p.Name);
                break;
        }
    }
}