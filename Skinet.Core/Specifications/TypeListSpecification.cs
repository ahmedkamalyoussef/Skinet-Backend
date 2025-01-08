using Skinet.Core.Entites;

namespace Skinet.Core.Specifications;

public class TypeListSpecification : BaseSpecification<Product, string>
{
    public TypeListSpecification()
    {
        SetSelect(product => product.Type);
        SetIsDistinctable();
    }
}