namespace InventoryManagementSystem.Api.Entities;

public sealed class Repair : BaseEntity
{
    public int AssetId { get; set; }

    public string Vendor { get; set; } = string.Empty;

    public decimal? Cost { get; set; }

    public string? Notes { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.Logged;

    public DateTime LoggedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public Asset? Asset { get; set; }
}
