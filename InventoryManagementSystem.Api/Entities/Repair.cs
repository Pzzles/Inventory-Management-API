namespace InventoryManagementSystem.Api.Entities;

public sealed class Repair : BaseEntity
{
    public string Description { get; set; } = string.Empty;

    public DateTime RepairDateUtc { get; set; }
}
