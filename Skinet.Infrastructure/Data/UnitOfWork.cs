using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Skinet.Infrastructure.Repositories;
using System.Collections.Concurrent;

namespace Skinet.Infrastructure.Data
{
    public class UnitOfWork(StoreContext _context) : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, object> _repositories = new();
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t =>
            {
                var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
                return Activator.CreateInstance(repositoryType, _context) ?? throw new InvalidOperationException($"could not create instance of {repositoryType}");
            });
        }
    }
}