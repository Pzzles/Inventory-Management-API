using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Contracts;

public interface IRepairService
{
    Task<ServiceResult<Repair>> CreateAsync(Repair repair, CancellationToken cancellationToken);

    Task<ServiceResult<Repair>> UpdateAsync(int id, Repair repair, CancellationToken cancellationToken);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<Repair?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Repair>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);
}
