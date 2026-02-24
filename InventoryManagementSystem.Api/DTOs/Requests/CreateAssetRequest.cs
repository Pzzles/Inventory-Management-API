using System.ComponentModel.DataAnnotations;
using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class CreateAssetRequest
{
    [Required]
    public string AssetTag { get; set; } = string.Empty;

    [Required]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateTime? WarrantyExpiry { get; set; }

    [Required]
    public AssetStatus Status { get; set; }

    [Required]
    public int LocationId { get; set; }

    public int? SupplierId { get; set; }

    public int? AssignedEmployeeId { get; set; }

    public string? Notes { get; set; }

    [Required]
    public string OperatorName { get; set; } = string.Empty;
}
