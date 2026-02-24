using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _repository;

    public SupplierService(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<Supplier>> CreateAsync(Supplier supplier, CancellationToken cancellationToken)
    {
        var validation = Validate(supplier);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Supplier>.Fail(validation.Errors.ToArray());
        }

        var created = await _repository.AddAsync(supplier, cancellationToken);
        return ServiceResult<Supplier>.Ok(created);
    }

    public async Task<ServiceResult<Supplier>> UpdateAsync(
        int id,
        Supplier supplier,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Supplier>.Fail(new ServiceError("not_found", "Supplier not found."));
        }

        var validation = Validate(supplier);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Supplier>.Fail(validation.Errors.ToArray());
        }

        existing.Name = supplier.Name;
        existing.Code = supplier.Code;
        existing.Email = supplier.Email;
        existing.Phone = supplier.Phone;

        await _repository.UpdateAsync(existing, cancellationToken);
        return ServiceResult<Supplier>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Supplier not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Supplier?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<Supplier?>.Ok(supplier);
    }

    public async Task<ServiceResult<PagedResult<Supplier>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<Supplier>>.Ok(result);
    }

    private static ServiceResult Validate(Supplier supplier)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(supplier.Name))
        {
            errors.Add(new ServiceError("name_required", "Name is required."));
        }

        if (string.IsNullOrWhiteSpace(supplier.Code))
        {
            errors.Add(new ServiceError("code_required", "Code is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }
}
