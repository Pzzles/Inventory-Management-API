namespace InventoryManagementSystem.Api.Entities;

public sealed class AuditLog : BaseEntity
{
    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public DateTime OccurredAtUtc { get; set; }

    public string OperatorName { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string? Details { get; set; }
}
