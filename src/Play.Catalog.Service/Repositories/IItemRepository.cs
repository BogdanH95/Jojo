using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public interface IItemRepository
    {
        Task CreateAsync(Item item, CancellationToken ct = default);
        Task<IReadOnlyCollection<Item>> GetAllAsync(CancellationToken ct = default);
        Task<Item> GetAsync(Guid id, CancellationToken ct = default);
        Task RemoveAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(Item item, CancellationToken ct = default);
    }
}