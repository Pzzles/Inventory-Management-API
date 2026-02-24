using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class SwapAssetRequest
{
    [Required]
    public int OldAssetId { get; set; }

    [Required]
    public int NewAssetId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public string OperatorName { get; set; } = string.Empty;
}
