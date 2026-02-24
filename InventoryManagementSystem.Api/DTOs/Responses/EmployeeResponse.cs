namespace InventoryManagementSystem.Api.DTOs.Responses;

public sealed class EmployeeResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string StaffNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
