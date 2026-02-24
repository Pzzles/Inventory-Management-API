using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class CreateEmployeeRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string StaffNumber { get; set; } = string.Empty;

    public string? Email { get; set; }
}
