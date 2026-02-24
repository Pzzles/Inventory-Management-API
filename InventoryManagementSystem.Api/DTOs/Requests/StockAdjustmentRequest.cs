using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class StockAdjustmentRequest
{
    [Required]
    public int Quantity { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public string OperatorName { get; set; } = string.Empty;
}
