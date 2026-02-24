namespace InventoryManagementSystem.Api.Entities;

public sealed class AssetStatusHistory : BaseEntity
{
    public int AssetId { get; set; }

    public AssetStatus? FromStatus { get; set; }

    public AssetStatus ToStatus { get; set; }

    public DateTime ChangedAtUtc { get; set; }

    public string OperatorName { get; set; } = string.Empty;

    public Asset? Asset { get; set; }
}
