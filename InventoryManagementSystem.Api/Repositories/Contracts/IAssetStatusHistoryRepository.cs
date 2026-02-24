using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Repositories.Contracts;

public interface IAssetStatusHistoryRepository
{
    Task AddAsync(AssetStatusHistory history, CancellationToken cancellationToken);
}
