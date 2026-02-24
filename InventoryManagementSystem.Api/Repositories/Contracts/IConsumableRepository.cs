using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Repositories.Contracts;

public interface IConsumableRepository : IRepository<Consumable>
{
    Task<IReadOnlyList<Consumable>> GetLowStockAsync(CancellationToken cancellationToken);
}
