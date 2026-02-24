namespace InventoryManagementSystem.Api.Entities;

public sealed class Consumable : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;
}
