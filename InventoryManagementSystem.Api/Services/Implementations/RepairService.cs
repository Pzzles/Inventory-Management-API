using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class RepairService : IRepairService
{
    private readonly IRepairRepository _repository;

    public RepairService(IRepairRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<Repair>> CreateAsync(Repair repair, CancellationToken cancellationToken)
    {
        var validation = Validate(repair);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Repair>.Fail(validation.Errors.ToArray());
        }

        var created = await _repository.AddAsync(repair, cancellationToken);
        return ServiceResult<Repair>.Ok(created);
    }

    public async Task<ServiceResult<Repair>> UpdateAsync(
        int id,
        Repair repair,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Repair>.Fail(new ServiceError("not_found", "Repair not found."));
        }

        var validation = Validate(repair);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Repair>.Fail(validation.Errors.ToArray());
        }

        existing.Description = repair.Description;
        existing.RepairDateUtc = repair.RepairDateUtc;

        await _repository.UpdateAsync(existing, cancellationToken);
        return ServiceResult<Repair>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Repair not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Repair?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var repair = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<Repair?>.Ok(repair);
    }

    public async Task<ServiceResult<PagedResult<Repair>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<Repair>>.Ok(result);
    }

    private static ServiceResult Validate(Repair repair)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(repair.Description))
        {
            errors.Add(new ServiceError("description_required", "Description is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }
}
