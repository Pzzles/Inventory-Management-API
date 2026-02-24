namespace InventoryManagementSystem.Api.Entities;

public sealed class AssetAssignmentHistory : BaseEntity
{
    public int AssetId { get; set; }

    public int? FromEmployeeId { get; set; }

    public int ToEmployeeId { get; set; }

    public DateTime AssignedAtUtc { get; set; }

    public DateTime? ReturnedAtUtc { get; set; }

    public string OperatorName { get; set; } = string.Empty;

    public Asset? Asset { get; set; }

    public Employee? FromEmployee { get; set; }

    public Employee? ToEmployee { get; set; }
}
