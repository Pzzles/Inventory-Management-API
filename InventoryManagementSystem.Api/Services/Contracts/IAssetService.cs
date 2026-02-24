using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Contracts;

public interface IAssetService
{
    Task<ServiceResult<Asset>> CreateAsync(Asset asset, CancellationToken cancellationToken);

    Task<ServiceResult<Asset>> UpdateAsync(int id, Asset asset, CancellationToken cancellationToken);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<Asset?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Asset>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);
}
