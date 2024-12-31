using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Play.Common.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync(ct);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await dbCollection.Find(filter).ToListAsync(ct);
        }

        public async Task<T> GetAsync(Guid id, CancellationToken ct = default)
        {
            return await dbCollection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await dbCollection.AsQueryable().FirstOrDefaultAsync(filter, ct);
        }

        public async Task CreateAsync(T entity, CancellationToken ct = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(T));
            }

            await dbCollection.InsertOneAsync(entity, null, ct);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(T));
            }

            var filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity, cancellationToken: ct);
        }

        public async Task RemoveAsync(Guid id, CancellationToken ct = default)
        {
            var filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter, ct);
        }
    }
}