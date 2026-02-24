using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class ConsumableService : IConsumableService
{
    private readonly IConsumableRepository _repository;
    private readonly IConsumableAdjustmentRepository _adjustmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ConsumableService(
        IConsumableRepository repository,
        IConsumableAdjustmentRepository adjustmentRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _adjustmentRepository = adjustmentRepository;
        _dateTimeProvider = dateTimeProvider;
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
        existing.Category = consumable.Category;
        existing.Unit = consumable.Unit;
        existing.QuantityOnHand = consumable.QuantityOnHand;
        existing.ReorderLevel = consumable.ReorderLevel;
        existing.LocationId = consumable.LocationId;

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

    public async Task<ServiceResult<Consumable>> StockInAsync(
        int id,
        int quantity,
        string reason,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Consumable>.Fail(new ServiceError("not_found", "Consumable not found."));
        }

        var validation = ValidateAdjustment(quantity, reason, operatorName);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Consumable>.Fail(validation.Errors.ToArray());
        }

        existing.QuantityOnHand += quantity;
        await _repository.UpdateAsync(existing, cancellationToken);

        await RecordAdjustment(existing.Id, quantity, reason, operatorName, cancellationToken);
        return ServiceResult<Consumable>.Ok(existing);
    }

    public async Task<ServiceResult<Consumable>> StockOutAsync(
        int id,
        int quantity,
        string reason,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Consumable>.Fail(new ServiceError("not_found", "Consumable not found."));
        }

        var validation = ValidateAdjustment(quantity, reason, operatorName);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Consumable>.Fail(validation.Errors.ToArray());
        }

        if (existing.QuantityOnHand - quantity < 0)
        {
            return ServiceResult<Consumable>.Fail(new ServiceError(
                "insufficient_stock",
                "Stock out would result in negative quantity."));
        }

        existing.QuantityOnHand -= quantity;
        await _repository.UpdateAsync(existing, cancellationToken);

        await RecordAdjustment(existing.Id, -quantity, reason, operatorName, cancellationToken);
        return ServiceResult<Consumable>.Ok(existing);
    }

    public async Task<ServiceResult<IReadOnlyList<Consumable>>> GetLowStockAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetLowStockAsync(cancellationToken);
        return ServiceResult<IReadOnlyList<Consumable>>.Ok(items);
    }

    private static ServiceResult Validate(Consumable consumable)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(consumable.Name))
        {
            errors.Add(new ServiceError("name_required", "Name is required."));
        }

        if (string.IsNullOrWhiteSpace(consumable.Category))
        {
            errors.Add(new ServiceError("category_required", "Category is required."));
        }

        if (string.IsNullOrWhiteSpace(consumable.Unit))
        {
            errors.Add(new ServiceError("unit_required", "Unit is required."));
        }

        if (consumable.QuantityOnHand < 0)
        {
            errors.Add(new ServiceError("quantity_invalid", "Quantity on hand cannot be negative."));
        }

        if (consumable.ReorderLevel < 0)
        {
            errors.Add(new ServiceError("reorder_invalid", "Reorder level cannot be negative."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }

    private static ServiceResult ValidateAdjustment(int quantity, string reason, string operatorName)
    {
        var errors = new List<ServiceError>();

        if (quantity <= 0)
        {
            errors.Add(new ServiceError("quantity_required", "Quantity must be greater than zero."));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            errors.Add(new ServiceError("reason_required", "Reason is required."));
        }

        if (string.IsNullOrWhiteSpace(operatorName))
        {
            errors.Add(new ServiceError("operator_required", "Operator name is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }

    private async Task RecordAdjustment(
        int consumableId,
        int quantityChange,
        string reason,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var adjustment = new ConsumableAdjustment
        {
            ConsumableId = consumableId,
            QuantityChange = quantityChange,
            Reason = reason,
            OperatorName = operatorName,
            AdjustedAtUtc = _dateTimeProvider.UtcNow
        };

        await _adjustmentRepository.AddAsync(adjustment, cancellationToken);
    }
}
