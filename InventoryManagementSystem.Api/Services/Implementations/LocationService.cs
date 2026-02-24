using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class LocationService : ILocationService
{
    private readonly ILocationRepository _repository;

    public LocationService(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<Location>> CreateAsync(Location location, CancellationToken cancellationToken)
    {
        var validation = Validate(location);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Location>.Fail(validation.Errors.ToArray());
        }

        var created = await _repository.AddAsync(location, cancellationToken);
        return ServiceResult<Location>.Ok(created);
    }

    public async Task<ServiceResult<Location>> UpdateAsync(
        int id,
        Location location,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Location>.Fail(new ServiceError("not_found", "Location not found."));
        }

        var validation = Validate(location);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Location>.Fail(validation.Errors.ToArray());
        }

        existing.Name = location.Name;
        existing.Code = location.Code;
        existing.Description = location.Description;
        existing.AddressLine1 = location.AddressLine1;
        existing.City = location.City;
        existing.State = location.State;
        existing.Country = location.Country;

        await _repository.UpdateAsync(existing, cancellationToken);
        return ServiceResult<Location>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Location not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Location?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<Location?>.Ok(location);
    }

    public async Task<ServiceResult<PagedResult<Location>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<Location>>.Ok(result);
    }

    private static ServiceResult Validate(Location location)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(location.Name))
        {
            errors.Add(new ServiceError("name_required", "Name is required."));
        }

        if (string.IsNullOrWhiteSpace(location.Code))
        {
            errors.Add(new ServiceError("code_required", "Code is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }
}
