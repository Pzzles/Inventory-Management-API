namespace InventoryManagementSystem.Api.Entities;

public sealed class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}
