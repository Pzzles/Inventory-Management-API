using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Api.DTOs.Requests;

public sealed class UpdateLocationRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? AddressLine1 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }
}
