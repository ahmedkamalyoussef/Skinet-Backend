using System.Linq.Expressions;
using Skinet.Core.Interfaces;

namespace Skinet.Core.Specifications;

public class BaseSpecification<T>(Expression<Func<T,bool>>? _criteria) : ISpecification<T>
{
    protected BaseSpecification():this(null) {}
    public Expression<Func<T, bool>>? Criteria => _criteria;
    public bool IsDistinctable { get; private set; }
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    protected void SetOrderBy(Expression<Func<T, object>> orderBy)
    {
        OrderBy = orderBy;
    }

    protected void SetOrderByDescending(Expression<Func<T, object>> orderByDescending)
    {
        OrderByDescending = orderByDescending;
    }
    protected void SetIsDistinctable(){IsDistinctable=true;}
}

public class BaseSpecification<T, TResult>(Expression<Func<T, bool>> _criteria)
    : BaseSpecification<T>(_criteria), ISpecification<T, TResult>
{
    protected BaseSpecification(): this(null!) {}
    public Expression<Func<T, TResult>>? Select { get; private set; }

    protected void SetSelect(Expression<Func<T, TResult>> select)
    {
        Select = select;
    }
}