using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class UpdateSupplierRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}
