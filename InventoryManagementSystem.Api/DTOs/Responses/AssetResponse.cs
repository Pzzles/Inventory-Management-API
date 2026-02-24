using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.DTOs.Responses;

public sealed class AssetResponse
{
    public int Id { get; set; }

    public string AssetTag { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateTime? WarrantyExpiry { get; set; }

    public AssetStatus Status { get; set; }

    public int LocationId { get; set; }

    public int? SupplierId { get; set; }

    public int? AssignedEmployeeId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
