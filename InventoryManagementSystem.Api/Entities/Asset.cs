namespace InventoryManagementSystem.Api.Entities;

public sealed class Asset : BaseEntity
{
    public string AssetTag { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateTime? WarrantyExpiry { get; set; }

    public AssetStatus Status { get; set; } = AssetStatus.InStock;

    public int LocationId { get; set; }

    public int? SupplierId { get; set; }

    public int? AssignedEmployeeId { get; set; }

    public string? Notes { get; set; }

    public Location? Location { get; set; }

    public Supplier? Supplier { get; set; }

    public Employee? AssignedEmployee { get; set; }
}
