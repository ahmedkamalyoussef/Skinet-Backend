using Skinet.Core.Entites;

namespace Skinet.Core.Specifications;

public class BrandListSpecification : BaseSpecification<Product,string>
{
    public BrandListSpecification()
    {
        SetSelect(p=>p.Brand);
        SetIsDistinctable();
    }
}