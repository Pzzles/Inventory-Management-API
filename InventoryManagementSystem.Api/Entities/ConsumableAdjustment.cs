namespace InventoryManagementSystem.Api.Entities;

public sealed class ConsumableAdjustment : BaseEntity
{
    public int ConsumableId { get; set; }

    public int QuantityChange { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime AdjustedAtUtc { get; set; }

    public string OperatorName { get; set; } = string.Empty;

    public Consumable? Consumable { get; set; }
}
