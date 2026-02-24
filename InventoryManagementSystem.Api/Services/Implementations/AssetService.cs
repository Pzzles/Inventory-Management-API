using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Entities;
using InventoryManagementSystem.Api.Repositories.Contracts;
using InventoryManagementSystem.Api.Repositories.Models;
using InventoryManagementSystem.Api.Services;
using InventoryManagementSystem.Api.Services.Contracts;
using InventoryManagementSystem.Api.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Api.Services.Implementations;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;
    private readonly IAssetStatusHistoryRepository _statusHistoryRepository;
    private readonly IAssetAssignmentHistoryRepository _assignmentHistoryRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly InventoryDbContext _dbContext;

    public AssetService(
        IAssetRepository repository,
        IAssetStatusHistoryRepository statusHistoryRepository,
        IAssetAssignmentHistoryRepository assignmentHistoryRepository,
        IDateTimeProvider dateTimeProvider,
        InventoryDbContext dbContext)
    {
        _repository = repository;
        _statusHistoryRepository = statusHistoryRepository;
        _assignmentHistoryRepository = assignmentHistoryRepository;
        _dateTimeProvider = dateTimeProvider;
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<Asset>> CreateAsync(
        Asset asset,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var validation = Validate(asset, operatorName);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Asset>.Fail(validation.Errors.ToArray());
        }

        if (await _repository.AssetTagExistsAsync(asset.AssetTag, null, cancellationToken))
        {
            return ServiceResult<Asset>.Fail(new ServiceError("asset_tag_duplicate", "Asset tag must be unique."));
        }

        var created = await _repository.AddAsync(asset, cancellationToken);

        await RecordStatusHistory(created.Id, null, created.Status, operatorName, cancellationToken);
        return ServiceResult<Asset>.Ok(created);
    }

    public async Task<ServiceResult<Asset>> UpdateAsync(
        int id,
        Asset asset,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return ServiceResult<Asset>.Fail(new ServiceError("not_found", "Asset not found."));
        }

        var validation = Validate(asset, operatorName);
        if (!validation.IsSuccess)
        {
            return ServiceResult<Asset>.Fail(validation.Errors.ToArray());
        }

        if (await _repository.AssetTagExistsAsync(asset.AssetTag, id, cancellationToken))
        {
            return ServiceResult<Asset>.Fail(new ServiceError("asset_tag_duplicate", "Asset tag must be unique."));
        }

        if (!IsValidStatusTransition(existing.Status, asset.Status))
        {
            return ServiceResult<Asset>.Fail(new ServiceError(
                "invalid_status_transition",
                $"Cannot transition from {existing.Status} to {asset.Status}."));
        }

        var fromStatus = existing.Status;
        var statusChanged = fromStatus != asset.Status;

        existing.AssetTag = asset.AssetTag;
        existing.SerialNumber = asset.SerialNumber;
        existing.Type = asset.Type;
        existing.Brand = asset.Brand;
        existing.Model = asset.Model;
        existing.Description = asset.Description;
        existing.PurchaseDate = asset.PurchaseDate;
        existing.WarrantyExpiry = asset.WarrantyExpiry;
        existing.Status = asset.Status;
        existing.LocationId = asset.LocationId;
        existing.SupplierId = asset.SupplierId;
        existing.AssignedEmployeeId = asset.AssignedEmployeeId;
        existing.Notes = asset.Notes;

        await _repository.UpdateAsync(existing, cancellationToken);

        if (statusChanged)
        {
            await RecordStatusHistory(existing.Id, fromStatus, asset.Status, operatorName, cancellationToken);
        }

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

    public async Task<ServiceResult<Asset>> AssignAsync(
        int assetId,
        int employeeId,
        string operatorName,
        CancellationToken cancellationToken)
    {
        if (employeeId <= 0)
        {
            return ServiceResult<Asset>.Fail(new ServiceError("employee_required", "Employee is required."));
        }

        if (string.IsNullOrWhiteSpace(operatorName))
        {
            return ServiceResult<Asset>.Fail(new ServiceError("operator_required", "Operator name is required."));
        }

        var asset = await _repository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<Asset>.Fail(new ServiceError("not_found", "Asset not found."));
        }

        if (asset.Status != AssetStatus.InStock)
        {
            return ServiceResult<Asset>.Fail(new ServiceError(
                "invalid_status",
                "Asset must be InStock to assign."));
        }

        var previousEmployeeId = asset.AssignedEmployeeId;
        asset.AssignedEmployeeId = employeeId;
        asset.Status = AssetStatus.Assigned;

        await _repository.UpdateAsync(asset, cancellationToken);

        await RecordAssignmentHistory(
            asset.Id,
            previousEmployeeId,
            employeeId,
            operatorName,
            cancellationToken);

        await RecordStatusHistory(asset.Id, AssetStatus.InStock, AssetStatus.Assigned, operatorName, cancellationToken);

        return ServiceResult<Asset>.Ok(asset);
    }

    public async Task<ServiceResult<Asset>> ReturnAsync(
        int assetId,
        string operatorName,
        AssetStatus returnStatus,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(operatorName))
        {
            return ServiceResult<Asset>.Fail(new ServiceError("operator_required", "Operator name is required."));
        }

        var asset = await _repository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<Asset>.Fail(new ServiceError("not_found", "Asset not found."));
        }

        if (asset.Status != AssetStatus.Assigned)
        {
            return ServiceResult<Asset>.Fail(new ServiceError(
                "invalid_status",
                "Asset must be Assigned to return."));
        }

        if (!IsValidStatusTransition(asset.Status, returnStatus))
        {
            return ServiceResult<Asset>.Fail(new ServiceError(
                "invalid_status_transition",
                $"Cannot transition from {asset.Status} to {returnStatus}."));
        }

        if (!asset.AssignedEmployeeId.HasValue)
        {
            return ServiceResult<Asset>.Fail(new ServiceError(
                "employee_required",
                "Assigned employee is required to return the asset."));
        }

        var fromStatus = asset.Status;
        asset.Status = returnStatus;
        var assignedEmployeeId = asset.AssignedEmployeeId;
        asset.AssignedEmployeeId = null;

        await _repository.UpdateAsync(asset, cancellationToken);

        await RecordReturnHistory(asset.Id, assignedEmployeeId.Value, operatorName, cancellationToken);

        await RecordStatusHistory(asset.Id, fromStatus, returnStatus, operatorName, cancellationToken);

        return ServiceResult<Asset>.Ok(asset);
    }

    public async Task<ServiceResult> SwapAsync(
        int oldAssetId,
        int newAssetId,
        int employeeId,
        string operatorName,
        CancellationToken cancellationToken)
    {
        if (oldAssetId == newAssetId)
        {
            return ServiceResult.Fail(new ServiceError("invalid_swap", "Old and new asset cannot be the same."));
        }

        if (employeeId <= 0)
        {
            return ServiceResult.Fail(new ServiceError("employee_required", "Employee is required."));
        }

        if (string.IsNullOrWhiteSpace(operatorName))
        {
            return ServiceResult.Fail(new ServiceError("operator_required", "Operator name is required."));
        }

        var oldAsset = await _repository.GetByIdAsync(oldAssetId, cancellationToken);
        if (oldAsset is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "Old asset not found."));
        }

        var newAsset = await _repository.GetByIdAsync(newAssetId, cancellationToken);
        if (newAsset is null)
        {
            return ServiceResult.Fail(new ServiceError("not_found", "New asset not found."));
        }

        if (oldAsset.Status != AssetStatus.Assigned || oldAsset.AssignedEmployeeId != employeeId)
        {
            return ServiceResult.Fail(new ServiceError(
                "invalid_old_asset",
                "Old asset must be assigned to the specified employee."));
        }

        if (newAsset.Status != AssetStatus.InStock)
        {
            return ServiceResult.Fail(new ServiceError(
                "invalid_new_asset",
                "New asset must be InStock."));
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var oldEmployeeId = oldAsset.AssignedEmployeeId;
        oldAsset.Status = AssetStatus.InStock;
        oldAsset.AssignedEmployeeId = null;
        await _repository.UpdateAsync(oldAsset, cancellationToken);
        if (oldEmployeeId.HasValue)
        {
            await RecordReturnHistory(oldAsset.Id, oldEmployeeId.Value, operatorName, cancellationToken);
        }
        await RecordStatusHistory(oldAsset.Id, AssetStatus.Assigned, AssetStatus.InStock, operatorName, cancellationToken);

        newAsset.Status = AssetStatus.Assigned;
        newAsset.AssignedEmployeeId = employeeId;
        await _repository.UpdateAsync(newAsset, cancellationToken);
        await RecordAssignmentHistory(newAsset.Id, null, employeeId, operatorName, cancellationToken);
        await RecordStatusHistory(newAsset.Id, AssetStatus.InStock, AssetStatus.Assigned, operatorName, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return ServiceResult.Ok();
    }

    private static ServiceResult Validate(Asset asset, string operatorName)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(asset.AssetTag))
        {
            errors.Add(new ServiceError("asset_tag_required", "Asset tag is required."));
        }

        if (string.IsNullOrWhiteSpace(asset.SerialNumber))
        {
            errors.Add(new ServiceError("serial_number_required", "Serial number is required."));
        }

        if (string.IsNullOrWhiteSpace(asset.Type))
        {
            errors.Add(new ServiceError("type_required", "Type is required."));
        }

        if (string.IsNullOrWhiteSpace(asset.Brand))
        {
            errors.Add(new ServiceError("brand_required", "Brand is required."));
        }

        if (string.IsNullOrWhiteSpace(asset.Model))
        {
            errors.Add(new ServiceError("model_required", "Model is required."));
        }

        if (asset.LocationId <= 0)
        {
            errors.Add(new ServiceError("location_required", "Location is required."));
        }

        if (string.IsNullOrWhiteSpace(operatorName))
        {
            errors.Add(new ServiceError("operator_required", "Operator name is required."));
        }

        return errors.Count == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail(errors.ToArray());
    }

    private static bool IsValidStatusTransition(AssetStatus fromStatus, AssetStatus toStatus)
    {
        if (fromStatus == toStatus)
        {
            return true;
        }

        return fromStatus switch
        {
            AssetStatus.InStock => toStatus is AssetStatus.Assigned
                or AssetStatus.InRepair
                or AssetStatus.Retired
                or AssetStatus.LostStolen,
            AssetStatus.Assigned => toStatus is AssetStatus.InStock
                or AssetStatus.InRepair
                or AssetStatus.Retired
                or AssetStatus.LostStolen,
            AssetStatus.InRepair => toStatus is AssetStatus.InStock
                or AssetStatus.Assigned
                or AssetStatus.Retired
                or AssetStatus.LostStolen,
            AssetStatus.Retired => false,
            AssetStatus.LostStolen => false,
            _ => false
        };
    }

    private async Task RecordStatusHistory(
        int assetId,
        AssetStatus? fromStatus,
        AssetStatus toStatus,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var history = new AssetStatusHistory
        {
            AssetId = assetId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ChangedAtUtc = _dateTimeProvider.UtcNow,
            OperatorName = operatorName
        };

        await _statusHistoryRepository.AddAsync(history, cancellationToken);
    }

    private async Task RecordAssignmentHistory(
        int assetId,
        int? fromEmployeeId,
        int toEmployeeId,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var history = new AssetAssignmentHistory
        {
            AssetId = assetId,
            FromEmployeeId = fromEmployeeId,
            ToEmployeeId = toEmployeeId,
            AssignedAtUtc = _dateTimeProvider.UtcNow,
            OperatorName = operatorName
        };

        await _assignmentHistoryRepository.AddAsync(history, cancellationToken);
    }

    private async Task RecordReturnHistory(
        int assetId,
        int employeeId,
        string operatorName,
        CancellationToken cancellationToken)
    {
        var history = new AssetAssignmentHistory
        {
            AssetId = assetId,
            FromEmployeeId = null,
            ToEmployeeId = employeeId,
            AssignedAtUtc = _dateTimeProvider.UtcNow,
            ReturnedAtUtc = _dateTimeProvider.UtcNow,
            OperatorName = operatorName
        };

        await _assignmentHistoryRepository.AddAsync(history, cancellationToken);
    }
}
