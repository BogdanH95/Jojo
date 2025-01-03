namespace Play.Inventory.Service.Dtos
{
    public record GrantItemDto(Guid UserId, Guid CatalogIdemId, int Quantity);

    public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);

    public record CatalogItemDto(Guid Id, string Name, string Description);
}