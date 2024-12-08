using Skinet.Core.Entites;

namespace Skinet.Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T?> GetEntityWithSpecification(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<TResult?> GetEntityWithSpecification<TResult>(ISpecification<T,TResult> spec);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T,TResult> spec);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> SaveAsync();
    bool Exists(int id);
}