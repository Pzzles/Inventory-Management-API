using System.ComponentModel.DataAnnotations;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class CreateRepairRequest
{
    [Required]
    public int AssetId { get; set; }

    [Required]
    public string Vendor { get; set; } = string.Empty;

    public decimal? Cost { get; set; }

    public string? Notes { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.Logged;

    [Required]
    public string OperatorName { get; set; } = string.Empty;
}
