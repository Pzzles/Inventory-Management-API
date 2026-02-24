using InventoryManagementSystem.Api.Entities;

namespace InventoryManagementSystem.Api.Repositories.Contracts;

public interface IAssetRepository : IRepository<Asset>
{
    Task<bool> AssetTagExistsAsync(string assetTag, int? excludeId, CancellationToken cancellationToken);
}
