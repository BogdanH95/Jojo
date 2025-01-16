using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer(IRepository<CatalogItem> repository) : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> repository = repository;

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(message.ItemId);

            if (item == null) return;

            await repository.RemoveAsync(message.ItemId);
        }
    }
}