using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public sealed class ConsumableRepository : Repository<Consumable>, IConsumableRepository
{
    public ConsumableRepository(InventoryDbContext context, IDateTimeProvider dateTimeProvider)
        : base(context, dateTimeProvider)
    {
    }

    public async Task<IReadOnlyList<Consumable>> GetLowStockAsync(CancellationToken cancellationToken)
    {
        return await Entities
            .Where(consumable => consumable.QuantityOnHand <= consumable.ReorderLevel)
            .ToListAsync(cancellationToken);
    }
}
