namespace InventoryManagementSystem.Api.Entities;

public sealed class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string StaffNumber { get; set; } = string.Empty;
}
