using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public sealed class AssetStatusHistoryRepository : IAssetStatusHistoryRepository
{
    private readonly InventoryDbContext _context;

    public AssetStatusHistoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AssetStatusHistory history, CancellationToken cancellationToken)
    {
        _context.AssetStatusHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
