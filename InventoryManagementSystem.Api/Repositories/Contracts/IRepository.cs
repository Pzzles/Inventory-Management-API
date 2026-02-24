using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Models;

namespace InventoryManagementSystem.Api.Repositories.Contracts;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<PagedResult<TEntity>> GetPagedAsync(RepositoryQueryOptions options, CancellationToken cancellationToken);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken);
}
