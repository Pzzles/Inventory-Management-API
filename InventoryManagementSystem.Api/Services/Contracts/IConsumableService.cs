using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Contracts;

public interface IConsumableService
{
    Task<ServiceResult<Consumable>> CreateAsync(Consumable consumable, CancellationToken cancellationToken);

    Task<ServiceResult<Consumable>> UpdateAsync(int id, Consumable consumable, CancellationToken cancellationToken);

    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<Consumable?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ServiceResult<PagedResult<Consumable>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken);

    Task<ServiceResult<Consumable>> StockInAsync(
        int id,
        int quantity,
        string reason,
        string operatorName,
        CancellationToken cancellationToken);

    Task<ServiceResult<Consumable>> StockOutAsync(
        int id,
        int quantity,
        string reason,
        string operatorName,
        CancellationToken cancellationToken);

    Task<ServiceResult<IReadOnlyList<Consumable>>> GetLowStockAsync(CancellationToken cancellationToken);
}
