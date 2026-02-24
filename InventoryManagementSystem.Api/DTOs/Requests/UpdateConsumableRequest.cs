using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class UpdateConsumableRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Unit { get; set; } = string.Empty;

    [Required]
    public int QuantityOnHand { get; set; }

    [Required]
    public int ReorderLevel { get; set; }

    public int? LocationId { get; set; }
}
