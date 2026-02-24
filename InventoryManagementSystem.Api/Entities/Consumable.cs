namespace InventoryManagementSystem.Api.Entities;

public sealed class Consumable : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public int QuantityOnHand { get; set; }

    public int ReorderLevel { get; set; }

    public int? LocationId { get; set; }

    public Location? Location { get; set; }
}
