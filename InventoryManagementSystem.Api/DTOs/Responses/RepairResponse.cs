using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.DTOs.Responses;

public sealed class RepairResponse
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public string Vendor { get; set; } = string.Empty;

    public decimal? Cost { get; set; }

    public string? Notes { get; set; }

    public RepairStatus Status { get; set; }

    public DateTime LoggedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
