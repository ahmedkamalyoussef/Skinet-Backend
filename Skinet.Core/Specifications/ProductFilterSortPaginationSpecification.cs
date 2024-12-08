using Skinet.Core.Entites;

namespace Skinet.Core.Specifications;

public class ProductFilterSortPaginationSpecification:BaseSpecification<Product>
{
    public ProductFilterSortPaginationSpecification(string? brand, string? type,string? sortBy) : base(product =>
        (string.IsNullOrWhiteSpace(brand) || product.Brand == brand) &&
        (string.IsNullOrWhiteSpace(type) || product.Type == type)
    )
    {
        switch (sortBy)
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