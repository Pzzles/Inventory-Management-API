using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class ConsumableService : IConsumableService
{
    private readonly IConsumableRepository _repository;

    public ConsumableService(IConsumableRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<Consumable>> CreateAsync(Consumable consumable, CancellationToken cancellationToken)
    {
        var validation = Validate(consumable);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Consumable>.Fail(validation.Errors.ToArray());
        }

        var created = await _repository.AddAsync(consumable, cancellationToken);
        return ServiceResult<Consumable>.Ok(created);
    }

    public async Task<ServiceResult<Consumable>> UpdateAsync(
        int id,
        Consumable consumable,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Consumable>.Fail(new ServiceError("not_found", "Consumable not found."));
        }

        var validation = Validate(consumable);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Consumable>.Fail(validation.Errors.ToArray());
        }

        existing.Name = consumable.Name;
        existing.Sku = consumable.Sku;

        await _repository.UpdateAsync(existing, cancellationToken);
        return ServiceResult<Consumable>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Consumable not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Consumable?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var consumable = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<Consumable?>.Ok(consumable);
    }

    public async Task<ServiceResult<PagedResult<Consumable>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<Consumable>>.Ok(result);
    }

    private static ServiceResult Validate(Consumable consumable)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(consumable.Name))
        {
            errors.Add(new ServiceError("name_required", "Name is required."));
        }

        if (string.IsNullOrWhiteSpace(consumable.Sku))
        {
            errors.Add(new ServiceError("sku_required", "SKU is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }
}
