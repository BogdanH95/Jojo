using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{

    public class ItemsRepository : IItemRepository
    {
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> dbCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public ItemsRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("Catalog");


            dbCollection = database.GetCollection<Item>(collectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync(CancellationToken ct = default)
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync(ct);
        }

        public async Task<Item> GetAsync(Guid id, CancellationToken ct = default)
        {
            return await dbCollection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task CreateAsync(Item item, CancellationToken ct = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await dbCollection.InsertOneAsync(item, null, ct);
        }

        public async Task UpdateAsync(Item item, CancellationToken ct = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await dbCollection.ReplaceOneAsync(filter, item, cancellationToken: ct);
        }

        public async Task RemoveAsync(Guid id, CancellationToken ct = default)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            await dbCollection.DeleteOneAsync(filter, ct);
        }
    }
}