using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Contracts;

public interface ISupplierService
{
    Task<ServiceResult<Supplier>> CreateAsync(Supplier supplier, CancellationToken cancellationToken);

    Task<ServiceResult<Supplier>> UpdateAsync(int id, Supplier supplier, CancellationToken cancellationToken);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<Supplier?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Supplier>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);
}
