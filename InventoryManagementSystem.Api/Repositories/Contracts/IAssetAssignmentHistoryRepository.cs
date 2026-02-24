using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Repositories.Contracts;

public interface IAssetAssignmentHistoryRepository
{
    Task AddAsync(AssetAssignmentHistory history, CancellationToken cancellationToken);
}
