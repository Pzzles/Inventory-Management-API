using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public sealed class ConsumableAdjustmentRepository : IConsumableAdjustmentRepository
{
    private readonly InventoryDbContext _context;

    public ConsumableAdjustmentRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ConsumableAdjustment adjustment, CancellationToken cancellationToken)
    {
        _context.ConsumableAdjustments.Add(adjustment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
