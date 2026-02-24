using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;

    public AssetService(IAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<Asset>> CreateAsync(Asset asset, CancellationToken cancellationToken)
    {
        var validation = Validate(asset);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Asset>.Fail(validation.Errors.ToArray());
        }

        var created = await _repository.AddAsync(asset, cancellationToken);
        return ServiceResult<Asset>.Ok(created);
    }

    public async Task<ServiceResult<Asset>> UpdateAsync(
        int id,
        Asset asset,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Asset>.Fail(new ServiceError("not_found", "Asset not found."));
        }

        var validation = Validate(asset);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Asset>.Fail(validation.Errors.ToArray());
        }

        existing.Name = asset.Name;
        existing.AssetTag = asset.AssetTag;

        await _repository.UpdateAsync(existing, cancellationToken);
        return ServiceResult<Asset>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Asset not found."));
        }

        await _repository.SoftDeleteAsync(existing, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Asset?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdAsync(id, cancellationToken);
        return ServiceResult<Asset?>.Ok(asset);
    }

    public async Task<ServiceResult<PagedResult<Asset>>> GetPagedAsync(
        RepositoryQueryOptions options,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(options, cancellationToken);
        return ServiceResult<PagedResult<Asset>>.Ok(result);
    }

    private static ServiceResult Validate(Asset asset)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(asset.Name))
        {
            errors.Add(new ServiceError("name_required", "Name is required."));
        }

        if (string.IsNullOrWhiteSpace(asset.AssetTag))
        {
            errors.Add(new ServiceError("asset_tag_required", "Asset tag is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }
}
