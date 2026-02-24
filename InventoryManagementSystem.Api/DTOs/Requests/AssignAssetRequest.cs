using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class AssignAssetRequest
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public string OperatorName { get; set; } = string.Empty;
}
