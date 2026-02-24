namespace InventoryManagementSystem.Api.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
