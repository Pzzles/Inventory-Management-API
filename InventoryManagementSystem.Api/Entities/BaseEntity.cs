namespace InventoryManagementSystem.Api.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public bool IsDeleted { get; set; }
}
