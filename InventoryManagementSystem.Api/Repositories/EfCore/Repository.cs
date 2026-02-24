using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Api.Repositories.EfCore;

public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected Repository(InventoryDbContext context, IDateTimeProvider dateTimeProvider)
    {
        Context = context;
        DateTimeProvider = dateTimeProvider;
        Entities = context.Set<TEntity>();
    }

    protected InventoryDbContext Context { get; }

    protected IDateTimeProvider DateTimeProvider { get; }

    protected DbSet<TEntity> Entities { get; }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Entities.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var query = Entities.AsQueryable();

        if (options.IncludeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        query = ApplyFilters(query, options);

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, options.PageNumber);
        var pageSize = Math.Max(1, options.PageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        entity.CreatedAtUtc = DateTimeProvider.UtcNow;
        entity.UpdatedAtUtc = null;
        entity.IsDeleted = false;

        Entities.Add(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        entity.UpdatedAtUtc = DateTimeProvider.UtcNow;
        Context.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTimeProvider.UtcNow;
        Context.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    protected virtual IQueryable<TEntity> ApplyFilters(
        IQueryable<TEntity> query,
        RepositoryQueryOptions options)
    {
        return query;
    }
}
