using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Repositories.Contracts;

public interface IConsumableAdjustmentRepository
{
    Task AddAsync(ConsumableAdjustment adjustment, CancellationToken cancellationToken);
}
