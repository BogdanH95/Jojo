using System.Linq.Expressions;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity, CancellationToken ct = default);
        Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
        Task<T> GetAsync(Guid id, CancellationToken ct = default);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
        Task RemoveAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
    }
}