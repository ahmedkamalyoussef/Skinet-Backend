using Microsoft.EntityFrameworkCore;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Skinet.Infrastructure.Data;

namespace Skinet.Infrastructure.Repositories;

public class GenericRepository<T>(StoreContext _context) : IGenericRepository<T> where T : BaseEntity
{
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetEntityWithSpecification(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }


    public bool Exists(int id)
    {
        return _context.Set<T>().Any(e => e.Id == id);
    }

    public async Task<TResult?> GetEntityWithSpecification<TResult>(ISpecification<T, TResult> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var query = _context.Set<T>().AsQueryable();
        query = spec.ApplyCriteria(query);
        return await query.CountAsync();
    }

    #region private methods
    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
    }

    private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> spec)
    {
        return SpecificationEvaluator<T>.GetQuery<T, TResult>(_context.Set<T>().AsQueryable(), spec);
    }
    #endregion
}