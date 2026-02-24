using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public sealed class AssetAssignmentHistoryRepository : IAssetAssignmentHistoryRepository
{
    private readonly InventoryDbContext _context;

    public AssetAssignmentHistoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AssetAssignmentHistory history, CancellationToken cancellationToken)
    {
        _context.AssetAssignmentHistories.Add(history);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
