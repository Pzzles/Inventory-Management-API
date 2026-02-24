namespace InventoryManagementSystem.Api.Entities;

public sealed class Asset : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string AssetTag { get; set; } = string.Empty;
}
